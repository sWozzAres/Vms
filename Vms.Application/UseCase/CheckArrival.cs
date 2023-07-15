using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Web.Shared;

namespace Vms.Application.UseCase;

public interface ICheckArrival
{
    Task CheckAsync(Guid id, TaskCheckArrivalCommand command, CancellationToken cancellationToken);
}

public class CheckArrival(VmsDbContext dbContext) : ICheckArrival
{
    readonly VmsDbContext DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    ServiceBookingRole? ServiceBooking;

    public async Task CheckAsync(Guid id, TaskCheckArrivalCommand command, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        switch (command.Result)
        {
            case TaskCheckArrivalCommand.TaskResult.Arrived:
                ServiceBooking.Arrived();
                break;
            case TaskCheckArrivalCommand.TaskResult.NotArrived:
                ServiceBooking.NotArrived();
                break;
            case TaskCheckArrivalCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule(Helper.CombineDateAndTime(command.RescheduleDate!.Value, command.RescheduleTime!.Value));
                break;
        }
    }

    class ServiceBookingRole(ServiceBooking self, CheckArrival ctx)
    {
        public void Arrived()
        {
            self.ChangeStatus(ServiceBookingStatus.CheckWorkStatus);
        }
        public void NotArrived()
        {
            self.ChangeStatus(ServiceBookingStatus.ChaseDriver);
        }
        public void Reschedule(DateTime rescheduleTime)
        {
            self.RescheduleTime = rescheduleTime;
        }
    }
}
