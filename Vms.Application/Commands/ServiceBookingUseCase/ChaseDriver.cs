using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public class ChaseDriver(
    VmsDbContext dbContext,
    IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<ChaseDriver> logger,
    ITimeService timeService) : ServiceBookingTaskBase(dbContext, activityLog)
{
    readonly ITimeService TimeService = timeService;
    ServiceBookingRole? ServiceBooking;
    TaskChaseDriverCommand Command = null!;

    public async Task ChaseAsync(Guid serviceBookingId, TaskChaseDriverCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Chasing driver for service booking: {servicebookingid}, command: {@taskchasedrivercommand}.", serviceBookingId, command);

        Command = command;
        ServiceBooking = new(await Load(serviceBookingId, cancellationToken), this);

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
                await ServiceBooking.Reschedule(Command.RescheduleReason!,
                    Command.RescheduleDate!.Value.ToDateTime(Command.RescheduleTime!.Value));
                break;
        }

        await LogActivity();
        taskLogger.Log(Id, nameof(ChaseDriver), Command);
    }

    class ServiceBookingRole(ServiceBooking self, ChaseDriver ctx) : ServiceBookingRoleBase<ChaseDriver>(self, ctx)
    {
        public void StillGoing()
        {
            Ctx.SummaryText.AppendLine("## Still Going");
            // TODO log when they expect to arrive then reschedule for that
            Self.ChangeStatus(ServiceBookingStatus.CheckArrival, Ctx.TimeService.Now.AddMinutes(30));
        }
        public void NotGoing()
        {
            Ctx.SummaryText.AppendLine("## Not Going");
            Self.ChangeStatus(ServiceBookingStatus.RebookDriver, Ctx.TimeService.Now);
        }
    }
}
