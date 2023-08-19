using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public class RebookDriver(
    VmsDbContext dbContext,
    IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<RebookDriver> logger,
    ITimeService timeService) : ServiceBookingTaskBase(dbContext, activityLog)
{
    readonly ITimeService TimeService = timeService;
    ServiceBookingRole? ServiceBooking;
    TaskRebookDriverCommand Command = null!;

    public async Task RebookAsync(Guid serviceBookingId, TaskRebookDriverCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Rebooking with driver for service booking: {servicebookingid}, command: {@taskrebookdrivercommand}.", serviceBookingId, command);

        Command = command;
        ServiceBooking = new(await Load(serviceBookingId, cancellationToken), this);

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
                await ServiceBooking.Reschedule(Command.RescheduleReason!,
                    Command.RescheduleDate!.Value.ToDateTime(Command.RescheduleTime!.Value));
                break;
        }

        await LogActivity();
        taskLogger.Log(Id, nameof(RebookDriver), Command);
    }

    class ServiceBookingRole(ServiceBooking self, RebookDriver ctx) : ServiceBookingRoleBase<RebookDriver>(self, ctx)
    {
        public void StillGoing()
        {
            Ctx.SummaryText.AppendLine("## Still Going");
            Self.Unbook();
            Self.Unassign();
            Self.ChangeStatus(ServiceBookingStatus.Assign, Ctx.TimeService.Now);
        }
        public void StillGoingToday()
        {
            var arrivalTime = Ctx.Command.ArrivalTime!.Value;

            Ctx.SummaryText.AppendLine("## Still Going Today");
            Ctx.SummaryText.AppendLine($"Will arrive at {arrivalTime}.");

            var date = Ctx.TimeService.Now.Date;
            var rescheduleTime = new DateTime(date.Year, date.Month, date.Day, arrivalTime.Hour, arrivalTime.Minute, arrivalTime.Second);
            Self.ChangeStatus(ServiceBookingStatus.CheckArrival, rescheduleTime);
        }
        public async Task NotGoing()
        {
            Ctx.SummaryText.AppendLine("## Not Going");

            // remove work items from booking
            if (Self.MotEventId.HasValue)
            {
                _ = await Ctx.DbContext.MotEvents.FindAsync(new object[] { Self.MotEventId }, Ctx.CancellationToken)
                    ?? throw new InvalidOperationException("Failed to load Mot Event.");
                Self.RemoveMotEvent();
            }

            Self.ChangeStatus(ServiceBookingStatus.Cancelled);
        }
    }
}
