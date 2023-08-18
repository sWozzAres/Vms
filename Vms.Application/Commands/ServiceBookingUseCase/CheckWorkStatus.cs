using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface ICheckWorkStatus
{
    Task CheckAsync(Guid serviceBookingId, TaskCheckWorkStatusCommand command, CancellationToken cancellationToken);
}

public class CheckWorkStatus(VmsDbContext dbContext, 
    IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ITimeService timeService,
    ILogger<CheckWorkStatus> logger) : ICheckWorkStatus
{
    readonly VmsDbContext DbContext = dbContext;
    readonly StringBuilder SummaryText = new();
    readonly ITimeService TimeService = timeService;
    ServiceBookingRole? ServiceBooking;
    Guid Id;
    TaskCheckWorkStatusCommand Command = null!;
    CancellationToken CancellationToken;

    public async Task CheckAsync(Guid serviceBookingId, TaskCheckWorkStatusCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking work status for service booking: {servicebookingid}, command: {@taskcheckworkstatuscommand}.", serviceBookingId, command);

        Id = serviceBookingId;
        Command = command ?? throw new ArgumentNullException(nameof(command));
        CancellationToken = cancellationToken;

        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { Id }, CancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

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
                await ServiceBooking.Reschedule();
                break;
        }

        if (!string.IsNullOrEmpty(Command.Callee))
            SummaryText.AppendLine($"* Callee: {Command.Callee}");

        _ = await activityLog.AddAsync(Id, SummaryText, CancellationToken);
        taskLogger.Log(Id, nameof(CheckWorkStatus), Command);
    }

    class ServiceBookingRole(ServiceBooking self, CheckWorkStatus ctx)
    {
        async Task CompleteAndRescheduleMot()
        {
            var motEvent = await ctx.DbContext.MotEvents
                    .SingleAsync(e => e.Id == self.MotEventId);// e.ServiceBookingId == self.Id && e.IsCurrent);

            motEvent.Complete();

            // next MOT is 1 year from either last MOT date or this MOT completion date, whichever is later
            var nextMotDate = ((ctx.Command.CompletionDate!.Value > motEvent.Due)
                ? ctx.Command.CompletionDate!.Value
                : motEvent.Due).AddYears(1);


            var nextMotEvent = new MotEvent(motEvent.CompanyCode, motEvent.VehicleId, nextMotDate, true);
            ctx.DbContext.MotEvents.Add(nextMotEvent);

            ctx.SummaryText.AppendLine($"* Next Mot scheduled for: {nextMotDate}");
        }
        public async Task Complete()
        {
            ctx.SummaryText.AppendLine("## Completed");
            ctx.SummaryText.AppendLine($"* Completed Date: {ctx.Command.CompletionDate}");

            if (self.MotEventId is not null)
                await CompleteAndRescheduleMot();

            self.ChangeStatus(ServiceBookingStatus.NotifyCustomer, ctx.TimeService.Now());
        }
        public async Task NotComplete()
        {
            ctx.SummaryText.AppendLine("## Not Complete");

            var nextChase = ctx.Command.NextChaseDate!.Value.ToDateTime(ctx.Command.NextChaseTime!.Value);
            ctx.SummaryText.AppendLine($"* Next Chase scheduled for: {nextChase.ToString("f")}");

            var reason = await ctx.DbContext.NotCompleteReasons.FindAsync(new object[] { self.CompanyCode, ctx.Command.NotCompleteReason! }, ctx.CancellationToken)
                ?? throw new InvalidOperationException("Failed to load not complete reason.");
            ctx.SummaryText.AppendLine($"* Reason Code: {reason.Code}");
            ctx.SummaryText.AppendLine($"* Reason Text: {reason.Name}");

            self.EstimatedCompletion = nextChase;
            self.ChangeStatus(ServiceBookingStatus.NotifyCustomerDelay, ctx.TimeService.Now());

        }
        public async Task Reschedule()
        {
            var reason = await ctx.DbContext.RescheduleReasons
                .SingleAsync(r => r.CompanyCode == self.CompanyCode && r.Code == ctx.Command.RescheduleReason!, ctx.CancellationToken);

            var rescheduleTime = ctx.Command.RescheduleDate!.Value.ToDateTime(ctx.Command.RescheduleTime!.Value);
            ctx.SummaryText.AppendLine("## Rescheduled");
            ctx.SummaryText.AppendLine($"* Time: {rescheduleTime.ToString("f")}");
            ctx.SummaryText.AppendLine($"* Reason Code: {reason.Code}");
            ctx.SummaryText.AppendLine($"* Reason Text: {reason.Name}");
            self.RescheduleTime = rescheduleTime;
        }
    }
}
