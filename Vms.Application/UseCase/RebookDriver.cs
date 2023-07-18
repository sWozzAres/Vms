using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Web.Shared;

namespace Vms.Application.UseCase;

public interface IRebookDriver
{
    Task RebookAsync(Guid id, TaskRebookDriverCommand command, CancellationToken cancellationToken);
}

public class RebookDriver(VmsDbContext context) : IRebookDriver
{
    readonly VmsDbContext DbContext = context ?? throw new ArgumentNullException(nameof(context));
    ServiceBookingRole? ServiceBooking;

    public async Task RebookAsync(Guid id, TaskRebookDriverCommand command, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        switch (command.Result)
        {
            case TaskRebookDriverCommand.TaskResult.StillGoing:
                ServiceBooking.StillGoing();
                break;
            case TaskRebookDriverCommand.TaskResult.NotGoing:
                ServiceBooking.NotGoing();
                break;
            case TaskRebookDriverCommand.TaskResult.StillGoingToday:
                ServiceBooking.StillGoingToday(Helper.CombineDateAndTime(DateOnly.FromDateTime(DateTime.Now), command.ArrivalTime!.Value));
                break;
            case TaskRebookDriverCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule(Helper.CombineDateAndTime(command.RescheduleDate!.Value, command.RescheduleTime!.Value));
                break;
        }
    }

    class ServiceBookingRole(ServiceBooking self, RebookDriver ctx)
    {
        public void StillGoing()
        {
            self.Unbook();
            self.Unassign();
        }
        public void StillGoingToday(DateTime arrivalTime)
        {
            self.RescheduleTime = arrivalTime;
            self.ChangeStatus(ServiceBookingStatus.CheckArrival);
        }
        public void NotGoing()
        {
            self.ChangeStatus(ServiceBookingStatus.Cancelled);
        }
        public void Reschedule(DateTime rescheduleTime)
        {
            self.RescheduleTime = rescheduleTime;
        }
    }
}
