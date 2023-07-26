using System.Text;
using Vms.Application.Services;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
using Vms.Web.Shared;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface ICheckWorkStatus
{
    Task CheckAsync(Guid id, TaskCheckWorkStatusCommand command, CancellationToken cancellationToken);
}

public class CheckWorkStatus(VmsDbContext dbContext, IActivityLogger activityLog, ITaskLogger taskLogger) : ICheckWorkStatus
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IActivityLogger ActivityLog = activityLog;
    readonly ITaskLogger TaskLogger = taskLogger;
    readonly StringBuilder SummaryText = new(); 
    ServiceBookingRole? ServiceBooking;

    public async Task CheckAsync(Guid id, TaskCheckWorkStatusCommand command, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        SummaryText.AppendLine("# Check Work Status");

        switch (command.Result)
        {
            case TaskCheckWorkStatusCommand.TaskResult.Complete:
                ServiceBooking.Complete();
                break;
            case TaskCheckWorkStatusCommand.TaskResult.NotComplete:
                ServiceBooking.NotComplete(Helper.CombineDateAndTime(command.NextChaseDate!.Value, command.NextChaseTime!.Value));
                break;
            case TaskCheckWorkStatusCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule(Helper.CombineDateAndTime(command.RescheduleDate!.Value, command.RescheduleTime!.Value), command.RescheduleReason!);
                break;
        }

        ActivityLog.Log(id, SummaryText);
        TaskLogger.Log(id, command);
    }

    class ServiceBookingRole(ServiceBooking self, CheckWorkStatus ctx)
    {
        public void Complete()
        {
            ctx.SummaryText.AppendLine("## Complete");
            self.ChangeStatus(ServiceBookingStatus.NotifyCustomer);
        }
        public void NotComplete(DateTime nextChase)
        {
            ctx.SummaryText.AppendLine("## Not Complete");
            self.EstimatedCompletion = nextChase;
            self.ChangeStatus(ServiceBookingStatus.NotifyCustomerDelay);

        }

        public void Reschedule(DateTime rescheduleTime, string reason)
        {
            ctx.SummaryText.AppendLine("## Rescheduled");
            ctx.SummaryText.AppendLine($"Rescheduled for {rescheduleTime.ToString("f")} because '{reason}'.");
            self.RescheduleTime = rescheduleTime;
        }
    }
}
