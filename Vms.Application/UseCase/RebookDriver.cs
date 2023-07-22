using System.Text;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Web.Shared;

namespace Vms.Application.UseCase;

public interface IRebookDriver
{
    Task RebookAsync(Guid id, TaskRebookDriverCommand command, CancellationToken cancellationToken);
}

public class RebookDriver(VmsDbContext context) : IRebookDriver
{
    readonly VmsDbContext DbContext = context ?? throw new ArgumentNullException(nameof(context));
    readonly StringBuilder SummaryText = new();
    ServiceBookingRole? ServiceBooking;

    public async Task RebookAsync(Guid id, TaskRebookDriverCommand command, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        SummaryText.AppendLine("Rebook Driver").AppendLine("=");

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
                ServiceBooking.Reschedule(Helper.CombineDateAndTime(command.RescheduleDate!.Value, command.RescheduleTime!.Value));
                break;
        }

        DbContext.ActivityLog.Add(new ActivityLog(id, SummaryText.ToString()));
    }

    class ServiceBookingRole(ServiceBooking self, RebookDriver ctx)
    {
        public void StillGoing()
        {
            ctx.SummaryText.AppendLine("Still Going").AppendLine("-");
            self.Unbook();
            self.Unassign();
        }
        public void StillGoingToday(TimeOnly arrivalTime)
        {
            ctx.SummaryText.AppendLine("Still Going Today").AppendLine("-");
            ctx.SummaryText.AppendLine($"Will arrive at {arrivalTime}.");

            self.RescheduleTime = Helper.CombineDateAndTime(DateOnly.FromDateTime(DateTime.Now), arrivalTime);
            self.ChangeStatus(ServiceBookingStatus.CheckArrival);
        }
        public void NotGoing()
        {
            ctx.SummaryText.AppendLine("Not Going").AppendLine("-");
            self.ChangeStatus(ServiceBookingStatus.Cancelled);
        }
        public void Reschedule(DateTime rescheduleTime)
        {
            self.RescheduleTime = rescheduleTime;
        }
    }
}
