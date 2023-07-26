using System.Text;
using Vms.Application.Services;
using Vms.Domain.Entity;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
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

    public async Task BookAsync(Guid id, TaskBookSupplierCommand command, CancellationToken cancellationToken)
    {
        ServiceBooking = await Load(id, cancellationToken);

        SummaryText.AppendLine("# Book Supplier");

        switch (command.Result)
        {
            case TaskBookSupplierCommand.TaskResult.Booked:
                await ServiceBooking.BookAsync(command.BookedDate!.Value, cancellationToken);
                break;
            case TaskBookSupplierCommand.TaskResult.Refused:
                await ServiceBooking.Refuse(command.RefusalReason!, cancellationToken);
                break;
            case TaskBookSupplierCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule(
                    Helper.CombineDateAndTime(command.RescheduleDate!.Value, command.RescheduleTime!.Value),
                    command.RescheduleReason!);
                break;
        }

        ActivityLog.Log(id, SummaryText);
        TaskLogger.Log(id, command);
    }

    async Task<ServiceBookingRole> Load(Guid id, CancellationToken cancellationToken)
        => new(await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

    class ServiceBookingRole(ServiceBooking self, BookSupplier ctx)
    {
        public async Task BookAsync(DateOnly bookedDate, CancellationToken cancellationToken)
        {
            ctx.SummaryText.AppendLine("## Booked");

            self.Book(bookedDate);

            var supplier = await ctx.DbContext.Suppliers.FindAsync(self.SupplierCode, cancellationToken)
                ?? throw new VmsDomainException("Failed to load supplier.");

            var drivers = await ctx.DbContext.DriverVehicles
                .Include(d => d.Driver)
                .Where(d => d.VehicleId == self.VehicleId)
                .Select(dv => dv.Driver)
                .ToListAsync(cancellationToken);

            var recipients = string.Join(";", drivers.Select(d => d.EmailAddress));
            ctx.EmailSender.Send(recipients, "Your service is booked",
                $"Your service is booked with {supplier.Name} on {bookedDate}.");
        }

        public async Task Refuse(string code, CancellationToken cancellationToken)
        {
            ctx.SummaryText.AppendLine("## Refused");

            if (self.SupplierCode is null)
                throw new VmsDomainException("Service Booking is not assigned.");

            // TODO
            var rr = await ctx.DbContext.RefusalReasons.FindAsync(new[] { self.CompanyCode, code }, cancellationToken)
                ?? throw new InvalidOperationException("Failed to load refusal reason.");

            //self.Supplier.RefusalReasonCode = rr.Code;
            //self.Supplier.RefusalReasonName = rr.Name;
            self.Unassign();
        }

        public void Reschedule(DateTime rescheduleTime, string reason)
        {
            ctx.SummaryText.AppendLine("## Rescheduled");
            ctx.SummaryText.AppendLine($"Rescheduled for {rescheduleTime.ToString("f")} because '{reason}'.");
            self.RescheduleTime = rescheduleTime;
        }
    }
}