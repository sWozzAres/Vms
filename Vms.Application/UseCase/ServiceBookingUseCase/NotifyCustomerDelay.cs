using System.Runtime.InteropServices.Marshalling;
using System.Text;
using Vms.Application.Services;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
using Vms.Web.Shared;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface INotifyCustomerDelay
{
    Task NotifyAsync(Guid id, TaskNotifyCustomerDelayCommand command, CancellationToken cancellationToken);
}

public class NotifyCustomerDelay(VmsDbContext context, IActivityLogger activityLog, ITaskLogger taskLogger) : INotifyCustomerDelay
{
    readonly VmsDbContext DbContext = context;
    readonly IActivityLogger ActivityLog = activityLog;
    readonly ITaskLogger TaskLogger = taskLogger;
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

        ActivityLog.Log(id, SummaryText);
        TaskLogger.Log(id, command);
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
