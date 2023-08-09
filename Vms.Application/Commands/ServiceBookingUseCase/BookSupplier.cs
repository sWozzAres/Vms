using Microsoft.Extensions.Logging;
using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface IBookSupplier
{
    Task BookAsync(Guid id, TaskBookSupplierCommand request, CancellationToken cancellationToken);
}

public class BookSupplier(VmsDbContext dbContext, IEmailSender emailSender, IActivityLogger activityLog,
    ITaskLogger taskLogger, ILogger<BookSupplier> logger) : IBookSupplier
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IEmailSender EmailSender = emailSender;
    readonly StringBuilder SummaryText = new();

    ServiceBookingRole? ServiceBooking;
    TaskBookSupplierCommand Command = null!;
    CancellationToken CancellationToken;
    Guid Id;

    public async Task BookAsync(Guid id, TaskBookSupplierCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("BookSupplier task for service booking {servicebookingid}, command: {@taskbooksuppliercommand}.", id, command);

        Id = id;
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
                ServiceBooking.Reschedule();
                break;
        }

        await activityLog.AddAsync(Id, SummaryText, CancellationToken);
        taskLogger.Log(Id, "Book Supplier", Command);
    }

    class ServiceBookingRole(ServiceBooking self, BookSupplier ctx)
    {
        public async Task BookAsync()
        {
            if (self.SupplierCode is null)
                throw new VmsDomainException("Service Booking is not assigned.");

            ctx.SummaryText.AppendLine("## Booked");

            self.Book(ctx.Command.BookedDate!.Value);

            await NotifyDriver();

            async Task NotifyDriver()
            {
                if (!string.IsNullOrEmpty(self.Driver.EmailAddress))
                {
                    var supplier = await ctx.DbContext.Suppliers.FindAsync(new object[] { self.SupplierCode }, ctx.CancellationToken)
                        ?? throw new VmsDomainException("Failed to find supplier.");

                    var recipient = self.Driver.EmailAddress;

                    ctx.EmailSender.Send(recipient, "Your service is booked",
                        $"Your service is booked with {supplier.Name} on {self.BookedDate}.");

                    ctx.SummaryText.AppendLine($"Notification email sent to driver at [{recipient}](mailto://{recipient}).");
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

        public void Reschedule()
        {
            var rescheduleTime = ctx.Command.RescheduleDate!.Value.ToDateTime(ctx.Command.RescheduleTime!.Value);
            ctx.SummaryText.AppendLine("## Rescheduled");
            ctx.SummaryText.AppendLine($"Rescheduled for {rescheduleTime.ToString("f")} because '{ctx.Command.RescheduleReason!}'.");
            self.RescheduleTime = rescheduleTime;
        }
    }
}