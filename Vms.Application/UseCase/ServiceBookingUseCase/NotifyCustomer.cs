using System.Text;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
using Vms.Web.Shared;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface INotifyCustomer
{
    Task NotifyAsync(Guid id, TaskNotifyCustomerCommand command, CancellationToken cancellationToken);
}

public class NotifyCustomer(VmsDbContext dbContext, IUserProvider userProvider) : INotifyCustomer
{
    readonly VmsDbContext DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    readonly IUserProvider UserProvider = userProvider;
    readonly StringBuilder SummaryText = new(); 
    ServiceBookingRole? ServiceBooking;

    public async Task NotifyAsync(Guid id, TaskNotifyCustomerCommand command, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        SummaryText.AppendLine("# Notify Customer");

        switch (command.Result)
        {
            case TaskNotifyCustomerCommand.TaskResult.Notified:
                ServiceBooking.Notify();
                break;
            case TaskNotifyCustomerCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule(Helper.CombineDateAndTime(command.RescheduleDate!.Value, command.RescheduleTime!.Value), command.RescheduleReason!);
                break;
        }

        DbContext.ActivityLog.Add(new ActivityLog(id, SummaryText.ToString(), UserProvider.UserId, UserProvider.UserName));
    }

    class ServiceBookingRole(ServiceBooking self, NotifyCustomer ctx)
    {
        public void Notify()
        {
            ctx.SummaryText.AppendLine("## Notify");
            self.ChangeStatus(ServiceBookingStatus.CheckArrival);
        }

        public void Reschedule(DateTime rescheduleTime, string reason)
        {
            ctx.SummaryText.AppendLine("## Rescheduled");
            ctx.SummaryText.AppendLine($"Rescheduled for {rescheduleTime.ToString("f")} because '{reason}'.");
            self.RescheduleTime = rescheduleTime;
        }
    }
}
