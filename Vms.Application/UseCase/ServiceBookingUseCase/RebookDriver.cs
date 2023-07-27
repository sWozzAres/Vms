using System.Text;
using Vms.Application.Services;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
using Vms.Web.Shared;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface IRebookDriver
{
    Task RebookAsync(Guid id, TaskRebookDriverCommand command, CancellationToken cancellationToken);
}

public class RebookDriver(VmsDbContext dbContext, IActivityLogger activityLog, ITaskLogger taskLogger) : IRebookDriver
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IActivityLogger ActivityLog = activityLog;
    readonly ITaskLogger TaskLogger = taskLogger;
    readonly StringBuilder SummaryText = new();
    ServiceBookingRole? ServiceBooking;

    public async Task RebookAsync(Guid id, TaskRebookDriverCommand command, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        SummaryText.AppendLine("# Rebook Driver");

        switch (command.Result)
        {
            case TaskRebookDriverCommand.TaskResult.StillGoing:
                ServiceBooking.StillGoing();
                break;
            case TaskRebookDriverCommand.TaskResult.NotGoing:
                ServiceBooking.NotGoing();
                break;
            case TaskRebookDriverCommand.TaskResult.StillGoingToday:
                ServiceBooking.StillGoingToday(command.ArrivalTime!.Value);
                break;
            case TaskRebookDriverCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule(
                    Helper.CombineDateAndTime(command.RescheduleDate!.Value, command.RescheduleTime!.Value),
                    command.RescheduleReason!
                );
                break;
        }

        await ActivityLog.LogAsync(id, SummaryText, cancellationToken);
        TaskLogger.Log(id, "Rebook Driver", command);
    }

    class ServiceBookingRole(ServiceBooking self, RebookDriver ctx)
    {
        public void StillGoing()
        {
            ctx.SummaryText.AppendLine("## Still Going");
            self.Unbook();
            self.Unassign();
        }
        public void StillGoingToday(TimeOnly arrivalTime)
        {
            ctx.SummaryText.AppendLine("## Still Going Today");
            ctx.SummaryText.AppendLine($"Will arrive at {arrivalTime}.");

            self.RescheduleTime = Helper.CombineDateAndTime(DateOnly.FromDateTime(DateTime.Now), arrivalTime);
            self.ChangeStatus(ServiceBookingStatus.CheckArrival);
        }
        public void NotGoing()
        {
            ctx.SummaryText.AppendLine("## Not Going");
            self.ChangeStatus(ServiceBookingStatus.Cancelled);
        }
        public void Reschedule(DateTime rescheduleTime, string reason)
        {
            ctx.SummaryText.AppendLine("## Rescheduled");
            ctx.SummaryText.AppendLine($"Rescheduled for {rescheduleTime.ToString("f")} because '{reason}'.");

            self.RescheduleTime = rescheduleTime;
        }
    }
}
