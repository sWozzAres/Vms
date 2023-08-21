using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public class CheckWorkStatus(
    VmsDbContext dbContext,
    IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ITimeService timeService,
    ILogger<CheckWorkStatus> logger) : ServiceBookingTaskBase(dbContext, activityLog)
{
    readonly ITimeService TimeService = timeService;
    ServiceBookingRole? ServiceBooking;
    TaskCheckWorkStatusCommand Command = null!;

    public async Task CheckAsync(Guid serviceBookingId, TaskCheckWorkStatusCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking work status for service booking: {servicebookingid}, command: {@taskcheckworkstatuscommand}.", serviceBookingId, command);

        Command = command;    
        ServiceBooking = new(await Load(serviceBookingId, cancellationToken), this);

        SummaryText.AppendLine("# Check Work Status");

        switch (command.Result)
        {
            case TaskCheckWorkStatusCommand.TaskResult.Complete:
                await ServiceBooking.Complete();
                break;
            case TaskCheckWorkStatusCommand.TaskResult.NotComplete:
                await ServiceBooking.NotComplete();
                break;
            case TaskCheckWorkStatusCommand.TaskResult.Rescheduled:
                await ServiceBooking.Reschedule(Command.RescheduleReason!,
                    Command.RescheduleDate!.Value.ToDateTime(Command.RescheduleTime!.Value));
                break;
        }

        if (!string.IsNullOrEmpty(Command.Callee))
            SummaryText.AppendLine($"* Callee: {Command.Callee}");

        await LogActivity();
        taskLogger.Log(Id, nameof(CheckWorkStatus), Command);
    }

    class ServiceBookingRole(ServiceBooking self, CheckWorkStatus ctx) : ServiceBookingRoleBase<CheckWorkStatus>(self, ctx)
    {
        async Task CompleteAndRescheduleMot()
        {
            var motEvent = await Ctx.DbContext.MotEvents
                    .SingleAsync(e => e.Id == Self.MotEventId);

            motEvent.Complete();

            // next MOT is 1 year from either last MOT date or this MOT completion date,
            // whichever is later
            var nextMotDate = ((Ctx.Command.CompletionDate!.Value > motEvent.Due)
                ? Ctx.Command.CompletionDate!.Value
                : motEvent.Due).AddYears(1);

            var nextMotEvent = new MotEvent(motEvent.CompanyCode, motEvent.VehicleId, nextMotDate);
            Ctx.DbContext.MotEvents.Add(nextMotEvent);

            Ctx.SummaryText.AppendLine($"* Next Mot scheduled for: {nextMotDate}");
        }
        public async Task Complete()
        {
            Ctx.SummaryText.AppendLine("## Completed");
            Ctx.SummaryText.AppendLine($"* Completed Date: {Ctx.Command.CompletionDate}");

            if (Self.MotEventId.HasValue)
                await CompleteAndRescheduleMot();

            Self.ChangeStatus(ServiceBookingStatus.NotifyCustomer, Ctx.TimeService.Now);
        }
        public async Task NotComplete()
        {
            Ctx.SummaryText.AppendLine("## Not Complete");

            var nextChase = Ctx.Command.NextChaseDate!.Value.ToDateTime(Ctx.Command.NextChaseTime!.Value);
            Ctx.SummaryText.AppendLine($"* Next Chase scheduled for: {nextChase.ToString("f")}");

            var reason = await Ctx.DbContext.NotCompleteReasons.AsNoTracking()
                .SingleAsync(r => r.CompanyCode == Self.CompanyCode && r.Code == Ctx.Command.NotCompleteReason!, Ctx.CancellationToken);

            Ctx.SummaryText.AppendLine($"* Reason Code: {reason.Code}");
            Ctx.SummaryText.AppendLine($"* Reason Text: {reason.Name}");

            Self.EstimatedCompletion = nextChase;
            Self.ChangeStatus(ServiceBookingStatus.NotifyCustomerDelay, Ctx.TimeService.Now);

        }
    }
}
