using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface IChaseDriver
{
    Task ChaseAsync(Guid id, TaskChaseDriverCommand command, CancellationToken cancellationToken);
}

public class ChaseDriver(VmsDbContext dbContext, IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<ChaseDriver> logger) : IChaseDriver
{
    readonly VmsDbContext DbContext = dbContext;
    readonly StringBuilder SummaryText = new();

    ServiceBookingRole? ServiceBooking;
    Guid Id;
    TaskChaseDriverCommand Command = null!;
    CancellationToken CancellationToken;

    public async Task ChaseAsync(Guid id, TaskChaseDriverCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Chasing driver for service booking: {servicebookingid}, command: {@taskchasedrivercommand}.", id, command);

        Id = id;
        Command = command ?? throw new ArgumentNullException(nameof(command));
        CancellationToken = cancellationToken;

        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { Id }, CancellationToken)
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
                await ServiceBooking.Reschedule();
                break;
        }

        _ = await activityLog.AddAsync(Id, SummaryText, CancellationToken);
        taskLogger.Log(Id, nameof(ChaseDriver), Command);
    }

    class ServiceBookingRole(ServiceBooking self, ChaseDriver ctx)
    {
        public void StillGoing()
        {
            ctx.SummaryText.AppendLine("## Still Going");
            // TODO log when they expect to arrive then reschedule for that
            self.ChangeStatus(ServiceBookingStatus.CheckArrival, DateTime.Now.AddMinutes(30));
        }
        public void NotGoing()
        {
            ctx.SummaryText.AppendLine("## Not Going");
            self.ChangeStatus(ServiceBookingStatus.RebookDriver, DateTime.Now);
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
