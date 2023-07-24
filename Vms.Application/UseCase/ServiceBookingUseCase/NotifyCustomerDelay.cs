using System.Runtime.InteropServices.Marshalling;
using System.Text;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
using Vms.Web.Shared;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface INotifyCustomerDelay
{
    Task NotifyAsync(Guid id, TaskNotifyCustomerDelayCommand command, CancellationToken cancellationToken);
}

public class NotifyCustomerDelay(VmsDbContext context, IUserProvider userProvider) : INotifyCustomerDelay
{
    readonly VmsDbContext DbContext = context ?? throw new ArgumentNullException(nameof(context));
    readonly IUserProvider UserProvider = userProvider;
    readonly StringBuilder SummaryText = new();
    ServiceBookingRole? ServiceBooking;

    public async Task NotifyAsync(Guid id, TaskNotifyCustomerDelayCommand command, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        SummaryText.AppendLine("# Notify Customer Delay");

        switch (command.Result)
        {
            case TaskNotifyCustomerDelayCommand.TaskResult.Notified:
                ServiceBooking.CustomerNotified();
                break;
            case TaskNotifyCustomerDelayCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule(
                    Helper.CombineDateAndTime(command.RescheduleDate!.Value, command.RescheduleTime!.Value),
                    command.RescheduleReason!
                );
                break;
        }

        DbContext.ActivityLog.Add(new ActivityLog(id, SummaryText.ToString(), UserProvider.UserId, UserProvider.UserName));
    }

    class ServiceBookingRole(ServiceBooking self, NotifyCustomerDelay ctx)
    {
        public void CustomerNotified()
        {
            ctx.SummaryText.AppendLine("## Customer Notified");
            self.RescheduleTime = self.EstimatedCompletion;
            self.ChangeStatus(ServiceBookingStatus.CheckWorkStatus);
        }

        public void Reschedule(DateTime rescheduleTime, string reason)
        {
            ctx.SummaryText.AppendLine("## Rescheduled");
            ctx.SummaryText.AppendLine($"Rescheduled for {rescheduleTime.ToString("f")} because '{reason}'.");

            self.RescheduleTime = rescheduleTime;
        }
    }
}
