using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Web.Shared;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Vms.Application.UseCase;

public interface ICheckWorkStatus
{
    Task CheckAsync(Guid id, TaskCheckWorkStatusCommand command, CancellationToken cancellationToken);
}

public class CheckWorkStatus(VmsDbContext context) : ICheckWorkStatus
{
    readonly VmsDbContext DbContext = context;
    ServiceBookingRole? ServiceBooking;

    public async Task CheckAsync(Guid id, TaskCheckWorkStatusCommand command, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        switch (command.Result)
        {
            case TaskCheckWorkStatusCommand.TaskResult.Complete:
                ServiceBooking.Complete();
                break;
            case TaskCheckWorkStatusCommand.TaskResult.NotComplete:
                ServiceBooking.NotComplete(Helper.CombineDateAndTime(command.NextChaseDate!.Value, command.NextChaseTime!.Value));
                break;
            case TaskCheckWorkStatusCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule(Helper.CombineDateAndTime(command.RescheduleDate!.Value, command.RescheduleTime!.Value));
                break;
        }
    }

    class ServiceBookingRole(ServiceBooking self, CheckWorkStatus context)
    {
        public void Complete()
        {
            self.ChangeStatus(ServiceBookingStatus.NotifyCustomer);
        }
        public void NotComplete(DateTime nextChase)
        {
            self.EstimatedCompletion = nextChase;
            self.ChangeStatus(ServiceBookingStatus.NotifyCustomerDelay);

        }

        public void Reschedule(DateTime rescheduleTime)
        {
            self.RescheduleTime = rescheduleTime;
        }
    }
}
