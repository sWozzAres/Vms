using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Web.Shared;

namespace Vms.Application.UseCase;

public interface IConfirmBooked
{
    Task ConfirmAsync(Guid id, TaskConfirmBookedCommand command, CancellationToken cancellationToken);
}

public class ConfirmBooked(VmsDbContext dbContext) : IConfirmBooked
{
    readonly VmsDbContext DBContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    ServiceBookingRole? ServiceBooking;

    public async Task ConfirmAsync(Guid id, TaskConfirmBookedCommand command, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DBContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        switch (command.Result)
        {
            case TaskConfirmBookedCommand.TaskResult.Confirmed:
                ServiceBooking.Confirm();
                break;
            case TaskConfirmBookedCommand.TaskResult.Refused:
                ServiceBooking.Refused(); break;
            case TaskConfirmBookedCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule(Helper.CombineDateAndTime(command.RescheduleDate!.Value, command.RescheduleTime!.Value));
                break;
        }
    }

    class ServiceBookingRole(ServiceBooking self, ConfirmBooked context)
    {
        public void Confirm()
        {
            self.ChangeStatus(ServiceBookingStatus.CheckArrival);
        }

        public void Refused()
        {
            self.Unbook();
            self.UnassignSupplier();

            //TODO
        }

        public void Reschedule(DateTime rescheduleTime)
        {
            self.RescheduleTime = rescheduleTime;
        }
    }
}
