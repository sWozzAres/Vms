using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface IBookSupplier
{
    Task BookAsync(Guid serviceBookingId, TaskBookSupplierCommand request, CancellationToken cancellationToken);
}

public class BookSupplier(VmsDbContext dbContext, IEmailSender<VmsDbContext> emailSender, IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger, ILogger<BookSupplier> logger) : IBookSupplier
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IEmailSender<VmsDbContext> EmailSender = emailSender;
    readonly StringBuilder SummaryText = new();
    readonly ILogger<BookSupplier> Logger = logger;

    ServiceBookingRole? ServiceBooking;
    TaskBookSupplierCommand Command = null!;
    CancellationToken CancellationToken;
    Guid Id;

    public async Task BookAsync(Guid serviceBookingId, TaskBookSupplierCommand command, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Booking supplier for service booking: {servicebookingid}, command: {@taskbooksuppliercommand}.", serviceBookingId, command);

        Id = serviceBookingId;
        Command = command ?? throw new ArgumentNullException(nameof(command));
        CancellationToken = cancellationToken;

        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { Id }, CancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

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
                await ServiceBooking.Reschedule();
                break;
        }

        if (!string.IsNullOrEmpty(Command.Callee))
            SummaryText.AppendLine($"* Callee: {Command.Callee}");

        _ = await activityLog.AddAsync(Id, SummaryText, CancellationToken);
        taskLogger.Log(Id, nameof(BookSupplier), Command);
    }

    class ServiceBookingRole(ServiceBooking self, BookSupplier ctx)
    {
        public async Task BookAsync()
        {
            if (self.SupplierCode is null)
                throw new VmsDomainException("Service Booking is not assigned.");

            ctx.SummaryText.AppendLine("## Booked");
            ctx.SummaryText.AppendLine($"* Booked Date: {ctx.Command.BookedDate}");

            self.Book(ctx.Command.BookedDate!.Value);

            await NotifyDriver();

            async Task NotifyDriver()
            {
                if (!string.IsNullOrEmpty(self.Driver.EmailAddress))
                {
                    ctx.Logger.LogInformation("Notifying driver {driveremailaddress}.", self.Driver.EmailAddress);

                    var supplier = await ctx.DbContext.Suppliers.FindAsync(new object[] { self.SupplierCode }, ctx.CancellationToken)
                        ?? throw new VmsDomainException("Failed to find supplier.");

                    var recipient = self.Driver.EmailAddress;

                    ctx.EmailSender.Send(recipient, "Your service is booked",
                        $"Your service is booked with '{supplier.Name}' on {self.BookedDate}.");

                    ctx.SummaryText.AppendLine($"* Notification email sent to driver at [{recipient}](mailto://{recipient}).");
                }
            }
        }

        public async Task Refuse()
        {
            ctx.SummaryText.AppendLine("## Refused");

            if (self.SupplierCode is null)
                throw new VmsDomainException("Service Booking is not assigned.");

            var reason = await ctx.DbContext.RefusalReasons.FindAsync(new object[] { self.CompanyCode, ctx.Command.RefusalReason! }, ctx.CancellationToken)
                ?? throw new InvalidOperationException("Failed to load refusal reason.");

            ctx.SummaryText.AppendLine($"* Reason Code: {reason.Code}");
            ctx.SummaryText.AppendLine($"* Reason Text: {reason.Name}");

            // log the refusal
            //var supplier = await ctx.DbContext.Suppliers.FindAsync(new object[] { self.SupplierCode }, ctx.CancellationToken);
            var supplierRefusal = new SupplierRefusal(self.SupplierCode, self.CompanyCode, reason.Code, reason.Name, self.Id);
            ctx.DbContext.SupplierRefusals.Add(supplierRefusal);

            self.Unassign();
        }

        public async Task Reschedule()
        {
            var reason = await ctx.DbContext.RescheduleReasons
                .SingleAsync(r => r.CompanyCode == self.CompanyCode && r.Code == ctx.Command.RescheduleReason!, ctx.CancellationToken);

            var rescheduleTime = ctx.Command.RescheduleDate!.Value.ToDateTime(ctx.Command.RescheduleTime!.Value);
            ctx.SummaryText.AppendLine("## Rescheduled");
            ctx.SummaryText.AppendLine($"* Time: {rescheduleTime.ToString("f")}");
            ctx.SummaryText.AppendLine($"* Reason Code: {reason.Code}");
            ctx.SummaryText.AppendLine($"* Reason Text: {reason.Name}");
            self.RescheduleTime = rescheduleTime;
        }
    }
}