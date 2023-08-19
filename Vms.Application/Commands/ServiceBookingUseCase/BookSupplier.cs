using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public class BookSupplier(
    VmsDbContext dbContext,
    IEmailSender<VmsDbContext> emailSender,
    IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<BookSupplier> logger,
    ITimeService timeService) : ServiceBookingTaskBase(dbContext, activityLog)
{
    readonly ITimeService TimeService = timeService;
    readonly IEmailSender<VmsDbContext> EmailSender = emailSender;
    readonly ILogger<BookSupplier> Logger = logger;

    ServiceBookingRole? ServiceBooking;
    TaskBookSupplierCommand Command = null!;

    public async Task BookAsync(Guid serviceBookingId, TaskBookSupplierCommand command, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Booking supplier for service booking: {servicebookingid}, command: {@taskbooksuppliercommand}.", serviceBookingId, command);

        Command = command;
        ServiceBooking = new(await Load(serviceBookingId, cancellationToken), this);

        SummaryText.AppendLine("# Book Supplier");

        switch (Command.Result)
        {
            case TaskBookSupplierCommand.TaskResult.Booked:
                await ServiceBooking.BookAsync();
                break;
            case TaskBookSupplierCommand.TaskResult.Refused:
                await ServiceBooking.Refuse();
                break;
            case TaskBookSupplierCommand.TaskResult.Rescheduled:
                await ServiceBooking.Reschedule(Command.RescheduleReason!,
                    Command.RescheduleDate!.Value.ToDateTime(Command.RescheduleTime!.Value));
                break;
        }

        if (!string.IsNullOrEmpty(Command.Callee))
            SummaryText.AppendLine($"* Callee: {Command.Callee}");

        await LogActivity();
        taskLogger.Log(Id, nameof(BookSupplier), Command);
    }

    class ServiceBookingRole(ServiceBooking self, BookSupplier ctx) : ServiceBookingRoleBase<BookSupplier>(self, ctx)
    {
        public async Task BookAsync()
        {
            if (Self.SupplierCode is null)
                throw new VmsDomainException("Service Booking is not assigned.");

            Ctx.SummaryText.AppendLine("## Booked");
            Ctx.SummaryText.AppendLine($"* Booked Date: {Ctx.Command.BookedDate}");

            Self.Book(Ctx.Command.BookedDate!.Value);
            Self.ChangeStatus(ServiceBookingStatus.Confirm, Ctx.TimeService.Now);

            await NotifyDriver();

            async Task NotifyDriver()
            {
                if (!string.IsNullOrEmpty(Self.Driver.EmailAddress))
                {
                    Ctx.Logger.LogInformation("Notifying driver {driveremailaddress}.", Self.Driver.EmailAddress);

                    var supplier = await Ctx.DbContext.Suppliers.AsNoTracking()
                        .SingleAsync(s => s.Code == Self.SupplierCode, Ctx.CancellationToken);

                    var recipient = Self.Driver.EmailAddress;

                    Ctx.EmailSender.Send(recipient, "Your service is booked",
                        $"Your service is booked with '{supplier.Name}' on {Self.BookedDate}.");

                    Ctx.SummaryText.AppendLine($"* Notification email sent to driver at [{recipient}](mailto://{recipient}).");
                }
            }
        }

        public async Task Refuse()
        {
            Ctx.SummaryText.AppendLine("## Refused");

            if (Self.SupplierCode is null)
                throw new VmsDomainException("Service Booking is not assigned.");

            var reason = await Ctx.DbContext.RefusalReasons.AsNoTracking()
                .SingleAsync(r => r.CompanyCode == Self.CompanyCode && r.Code == Ctx.Command.RefusalReason!, Ctx.CancellationToken);

            Ctx.SummaryText.AppendLine($"* Reason Code: {reason.Code}");
            Ctx.SummaryText.AppendLine($"* Reason Text: {reason.Name}");

            // log the refusal
            var supplierRefusal = new SupplierRefusal(Self.SupplierCode, Self.CompanyCode, reason.Code, reason.Name, Self.Id);
            Ctx.DbContext.SupplierRefusals.Add(supplierRefusal);

            Self.Unassign();
            Self.ChangeStatus(ServiceBookingStatus.Assign, Ctx.TimeService.Now);
        }
    }
}