using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Web.Shared;

namespace Vms.Application.UseCase;

public interface INotifyCustomerDelay
{
    Task NotifyAsync(Guid id, TaskNotifyCustomerDelayCommand command, CancellationToken cancellationToken);
}

public class NotifyCustomerDelay(VmsDbContext dbContext) : INotifyCustomerDelay
{
    readonly VmsDbContext DBContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    ServiceBookingRole? ServiceBooking;

    public async Task NotifyAsync(Guid id, TaskNotifyCustomerDelayCommand command, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DBContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        switch (command.Result)
        {
            case TaskNotifyCustomerDelayCommand.TaskResult.Notified:
                ServiceBooking.CustomerNotified();
                break;
            case TaskNotifyCustomerDelayCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule(Helper.CombineDateAndTime(command.RescheduleDate!.Value, command.RescheduleTime!.Value));
                break;
        }
    }

    class ServiceBookingRole(ServiceBooking self, NotifyCustomerDelay context)
    {
        public void CustomerNotified()
        {
            self.RescheduleTime = self.EstimatedCompletion;
            self.ChangeStatus(ServiceBookingStatus.CheckWorkStatus);
        }

        public void Reschedule(DateTime rescheduleTime)
        {
            self.RescheduleTime = rescheduleTime;
        }
    }
}
