using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public class CheckArrival(
    VmsDbContext dbContext,
    IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<CheckArrival> logger,
    ITimeService timeService) : ServiceBookingTaskBase(dbContext, activityLog)
{
    readonly ITimeService TimeService = timeService;
    ServiceBookingRole? ServiceBooking;
    TaskCheckArrivalCommand Command = null!;

    public async Task CheckAsync(Guid serviceBookingId, TaskCheckArrivalCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking arrival for service booking: {servicebookingid}, command: {@taskcheckarrivalcommand}.", serviceBookingId, command);

        Command = command;
        ServiceBooking = new(await Load(serviceBookingId, cancellationToken), this);

        SummaryText.AppendLine("# Check Arrival");

        switch (Command.Result)
        {
            case TaskCheckArrivalCommand.TaskResult.Arrived:
                ServiceBooking.Arrived();
                break;
            case TaskCheckArrivalCommand.TaskResult.NotArrived:
                await ServiceBooking.NotArrived();
                break;
            case TaskCheckArrivalCommand.TaskResult.Rescheduled:
                await ServiceBooking.Reschedule(Command.RescheduleReason!,
                    Command.RescheduleDate!.Value.ToDateTime(Command.RescheduleTime!.Value));
                break;
        }

        if (!string.IsNullOrEmpty(Command.Callee))
            SummaryText.AppendLine($"* Callee: {Command.Callee}");

        await LogActivity();
        taskLogger.Log(Id, nameof(CheckArrival), Command);
    }

    class ServiceBookingRole(ServiceBooking self, CheckArrival ctx) : ServiceBookingRoleBase<CheckArrival>(self, ctx)
    {
        public void Arrived()
        {
            Ctx.SummaryText.AppendLine("## Arrived");

            var arrivalTime = Ctx.Command.ArrivalDate!.Value.ToDateTime(
                Ctx.Command.ArrivalTime!.Value);
            Ctx.SummaryText.AppendLine($"* Arrival Time: {arrivalTime.ToString("f")}");

            // schedule check work status for 4pm on the arrival date
            var rescheduleTime = Ctx.Command.ArrivalDate!.Value.ToDateTime(
                TimeOnly.FromTimeSpan(new TimeSpan(16, 0, 0)));
            Self.ChangeStatus(ServiceBookingStatus.CheckWorkStatus, rescheduleTime);
        }
        public async Task NotArrived()
        {
            Ctx.SummaryText.AppendLine("## Not Arrived");

            var reason = await Ctx.DbContext.NonArrivalReasons.AsNoTracking()
                .SingleAsync(r => r.CompanyCode == Self.CompanyCode && r.Code == Ctx.Command.NonArrivalReason!, Ctx.CancellationToken);

            Ctx.SummaryText.AppendLine($"* Reason Code: {reason.Code}");
            Ctx.SummaryText.AppendLine($"* Reason Text: {reason.Name}");

            if (Self.ServiceLevel == ServiceLevel.Collection || Self.ServiceLevel == ServiceLevel.Mobile)
                Self.ChangeStatus(ServiceBookingStatus.RebookDriver, Ctx.TimeService.Now);
            else
                Self.ChangeStatus(ServiceBookingStatus.ChaseDriver, Ctx.TimeService.Now);
        }
    }
}
