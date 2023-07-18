using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Web.Shared;

namespace Vms.Application.UseCase;

public interface IChaseDriver
{
    Task ChaseAsync(Guid id, TaskChaseDriverCommand command, CancellationToken cancellationToken);
}

public class ChaseDriver(VmsDbContext context) : IChaseDriver
{
    readonly VmsDbContext DbContext = context ?? throw new ArgumentNullException(nameof(context));
    ServiceBookingRole? ServiceBooking;

    public async Task ChaseAsync(Guid id, TaskChaseDriverCommand command, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        switch (command.Result)
        {
            case TaskChaseDriverCommand.TaskResult.StillGoing:
                ServiceBooking.StillGoing();
                break;
            case TaskChaseDriverCommand.TaskResult.NotGoing:
                ServiceBooking.NotGoing();
                break;
            case TaskChaseDriverCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule(Helper.CombineDateAndTime(command.RescheduleDate!.Value, command.RescheduleTime!.Value));
                break;
        }
    }

    class ServiceBookingRole(ServiceBooking self, ChaseDriver ctx)
    {
        public void StillGoing()
        {
            self.ChangeStatus(ServiceBookingStatus.CheckArrival);
        }
        public void NotGoing()
        {
            self.ChangeStatus(ServiceBookingStatus.RebookDriver);
        }
        public void Reschedule(DateTime rescheduleTime)
        {
            self.RescheduleTime = rescheduleTime;
        }
    }
}
