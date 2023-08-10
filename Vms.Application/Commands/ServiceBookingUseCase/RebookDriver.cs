using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface IRebookDriver
{
    Task RebookAsync(Guid id, TaskRebookDriverCommand command, CancellationToken cancellationToken);
}

public class RebookDriver(VmsDbContext dbContext, IActivityLogger activityLog, ITaskLogger taskLogger,
    ILogger<RebookDriver> logger) : IRebookDriver
{
    readonly VmsDbContext DbContext = dbContext;
    readonly StringBuilder SummaryText = new();

    ServiceBookingRole? ServiceBooking;
    Guid Id;
    TaskRebookDriverCommand Command = null!;
    CancellationToken CancellationToken;

    public async Task RebookAsync(Guid id, TaskRebookDriverCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Rebooking with driver for service booking: {servicebookingid}, command: {@taskrebookdrivercommand}.", id, command);

        Id = id;
        Command = command ?? throw new ArgumentNullException(nameof(command));
        CancellationToken = cancellationToken;

        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { Id }, CancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        SummaryText.AppendLine("# Rebook Driver");

        switch (command.Result)
        {
            case TaskRebookDriverCommand.TaskResult.StillGoing:
                ServiceBooking.StillGoing();
                break;
            case TaskRebookDriverCommand.TaskResult.NotGoing:
                await ServiceBooking.NotGoing();
                break;
            case TaskRebookDriverCommand.TaskResult.StillGoingToday:
                ServiceBooking.StillGoingToday();
                break;
            case TaskRebookDriverCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule();
                break;
        }

        await activityLog.AddAsync(Id, SummaryText, CancellationToken);
        taskLogger.Log(Id, "Rebook Driver", Command);
    }

    class ServiceBookingRole(ServiceBooking self, RebookDriver ctx)
    {
        public void StillGoing()
        {
            ctx.SummaryText.AppendLine("## Still Going");
            self.Unbook();
            self.Unassign();
        }
        public void StillGoingToday()
        {
            var arrivalTime = ctx.Command.ArrivalTime!.Value;

            ctx.SummaryText.AppendLine("## Still Going Today");
            ctx.SummaryText.AppendLine($"Will arrive at {arrivalTime}.");

            var date = DateTime.Today;
            var rescheduleTime = new DateTime(date.Year, date.Month, date.Day, arrivalTime.Hour, arrivalTime.Minute, arrivalTime.Second);
            self.ChangeStatus(ServiceBookingStatus.CheckArrival, rescheduleTime);
        }
        public async Task NotGoing()
        {
            ctx.SummaryText.AppendLine("## Not Going");

            // remove work items from booking

            var motEvent = await ctx.DbContext.MotEvents.SingleOrDefaultAsync(e => e.ServiceBookingId == self.Id && e.IsCurrent);
            if (motEvent is not null)
            {
                motEvent.ServiceBookingId = null;
            }

            self.ChangeStatus(ServiceBookingStatus.Cancelled);
        }

        public void Reschedule()
        {
            var rescheduleTime = ctx.Command.RescheduleDate!.Value.ToDateTime(ctx.Command.RescheduleTime!.Value);
            ctx.SummaryText.AppendLine("## Rescheduled");
            ctx.SummaryText.AppendLine($"Rescheduled for {rescheduleTime.ToString("f")} because '{ctx.Command.RescheduleReason!}'.");
            self.RescheduleTime = rescheduleTime;
        }
    }
}
