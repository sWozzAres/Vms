using System.Text;
using Vms.Application.Services;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
using Vms.Web.Shared;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface IChaseDriver
{
    Task ChaseAsync(Guid id, TaskChaseDriverCommand command, CancellationToken cancellationToken);
}

public class ChaseDriver(VmsDbContext dbContext, IActivityLogger activityLog, ITaskLogger taskLogger) : IChaseDriver
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IActivityLogger ActivityLog = activityLog;
    readonly ITaskLogger TaskLogger = taskLogger;
    readonly StringBuilder SummaryText = new();
    ServiceBookingRole? ServiceBooking;

    public async Task ChaseAsync(Guid id, TaskChaseDriverCommand command, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        SummaryText.AppendLine("# Chase Driver");

        switch (command.Result)
        {
            case TaskChaseDriverCommand.TaskResult.StillGoing:
                ServiceBooking.StillGoing();
                break;
            case TaskChaseDriverCommand.TaskResult.NotGoing:
                ServiceBooking.NotGoing();
                break;
            case TaskChaseDriverCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule(
                    Helper.CombineDateAndTime(command.RescheduleDate!.Value, command.RescheduleTime!.Value), command.RescheduleReason!);
                break;
        }

        ActivityLog.Log(id, SummaryText);
        TaskLogger.Log(id, command);
    }

    class ServiceBookingRole(ServiceBooking self, ChaseDriver ctx)
    {
        public void StillGoing()
        {
            ctx.SummaryText.AppendLine("## Still Going");
            self.ChangeStatus(ServiceBookingStatus.CheckArrival);
        }
        public void NotGoing()
        {
            ctx.SummaryText.AppendLine("## Not Going");
            self.ChangeStatus(ServiceBookingStatus.RebookDriver);
        }
        public void Reschedule(DateTime rescheduleTime, string reason)
        {
            ctx.SummaryText.AppendLine("## Rescheduled");
            ctx.SummaryText.AppendLine($"Rescheduled for {rescheduleTime.ToString("f")} because '{reason}'.");
            self.RescheduleTime = rescheduleTime;
        }
    }
}
