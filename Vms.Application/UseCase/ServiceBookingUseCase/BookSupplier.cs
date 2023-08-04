using System.Text;
using Vms.Application.Services;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.DomainApplication.Services;
using Vms.Web.Shared;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface IBookSupplier
{
    Task BookAsync(Guid id, TaskBookSupplierCommand request, CancellationToken cancellationToken);
}

public class BookSupplier(VmsDbContext dbContext, IEmailSender emailSender, IActivityLogger activityLog, ITaskLogger taskLogger) : IBookSupplier
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IEmailSender EmailSender = emailSender;
    readonly IActivityLogger ActivityLog = activityLog;
    readonly ITaskLogger TaskLogger = taskLogger;
    readonly StringBuilder SummaryText = new();
    
    ServiceBookingRole? ServiceBooking;
    TaskBookSupplierCommand Command = null!;
    CancellationToken CancellationToken;
    Guid Id;

    public async Task BookAsync(Guid id, TaskBookSupplierCommand command, CancellationToken cancellationToken)
    {
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

        await ActivityLog.AddAsync(Id, SummaryText, CancellationToken);
        TaskLogger.Log(Id, "Book Supplier", Command);
    }

    class ServiceBookingRole(ServiceBooking self, BookSupplier ctx)
    {
        public async Task BookAsync()
        {
            if (self.SupplierCode is null)
                throw new VmsDomainException("Service Booking is not assigned.");

            ctx.SummaryText.AppendLine("## Booked");

            self.BookedDate = ctx.Command.BookedDate!.Value;
            self.ChangeStatus(ServiceBookingStatus.Confirm, DateTime.Now);

            await NotifyDriver();

            async Task NotifyDriver()
            {
                var supplier = await ctx.DbContext.Suppliers.FindAsync(new object[] { self.SupplierCode }, ctx.CancellationToken)
                    ?? throw new VmsDomainException("Failed to load supplier.");

                var drivers = await ctx.DbContext.DriverVehicles
                    .Include(d => d.Driver)
                    .Where(d => d.VehicleId == self.VehicleId)
                    .Select(dv => dv.Driver)
                    .ToListAsync(ctx.CancellationToken);

                var recipients = string.Join(";", drivers.Select(d => d.EmailAddress));
                ctx.EmailSender.Send(recipients, "Your service is booked",
                    $"Your service is booked with {supplier.Name} on {self.BookedDate}.");
            }
        }

        public async Task Refuse()
        {
            ctx.SummaryText.AppendLine("## Refused");

            if (self.SupplierCode is null)
                throw new VmsDomainException("Service Booking is not assigned.");

            // TODO
            var rr = await ctx.DbContext.RefusalReasons.FindAsync(new object[] { self.CompanyCode, ctx.Command.RefusalReason! }, ctx.CancellationToken)
                ?? throw new InvalidOperationException("Failed to load refusal reason.");

            ctx.SummaryText.AppendLine($"* Reason Code: {rr.Code}");
            ctx.SummaryText.AppendLine($"* Reason Text: {rr.Name}");
            //self.Supplier.RefusalReasonCode = rr.Code;
            //self.Supplier.RefusalReasonName = rr.Name;
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