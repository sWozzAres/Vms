using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Web.Shared;

namespace Vms.Application.UseCase;

public interface INotifyCustomer
{
    Task NotifyAsync(Guid id, TaskNotifyCustomerCommand command, CancellationToken cancellationToken);
}

public class NotifyCustomer(VmsDbContext dbContext) : INotifyCustomer
{
    readonly VmsDbContext DBContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    ServiceBookingRole? ServiceBooking;

    public async Task NotifyAsync(Guid id, TaskNotifyCustomerCommand command, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DBContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        switch (command.Result)
        {
            case TaskNotifyCustomerCommand.TaskResult.Notified:
                ServiceBooking.Notify();
                break;
            case TaskNotifyCustomerCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule(Helper.CombineDateAndTime(command.RescheduleDate!.Value, command.RescheduleTime!.Value));
                break;
        }
    }

    class ServiceBookingRole(ServiceBooking self, NotifyCustomer context)
    {
        public void Notify()
        {
            self.ChangeStatus(ServiceBookingStatus.CheckArrival);
        }

        public void Reschedule(DateTime rescheduleTime)
        {
            self.RescheduleTime = rescheduleTime;
        }
    }
}
