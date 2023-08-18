using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface ICheckArrival
{
    Task CheckAsync(Guid serviceBookingId, TaskCheckArrivalCommand command, CancellationToken cancellationToken);
}

public class CheckArrival(VmsDbContext dbContext, IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<CheckArrival> logger) : ICheckArrival
{
    readonly VmsDbContext DbContext = dbContext;
    readonly StringBuilder SummaryText = new();

    ServiceBookingRole? ServiceBooking;
    Guid Id;
    TaskCheckArrivalCommand Command = null!;
    CancellationToken CancellationToken;

    public async Task CheckAsync(Guid serviceBookingId, TaskCheckArrivalCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking arrival for service booking: {servicebookingid}, command: {@taskcheckarrivalcommand}.", serviceBookingId, command);

        Id = serviceBookingId;
        Command = command ?? throw new ArgumentNullException(nameof(command));
        CancellationToken = cancellationToken;

        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { Id }, CancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

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
                await ServiceBooking.Reschedule();
                break;
        }

        if (!string.IsNullOrEmpty(Command.Callee))
            SummaryText.AppendLine($"* Callee: {Command.Callee}");

        _ = await activityLog.AddAsync(Id, SummaryText, CancellationToken);
        taskLogger.Log(Id, nameof(CheckArrival), Command);
    }

    class ServiceBookingRole(ServiceBooking self, CheckArrival ctx)
    {
        public void Arrived()
        {
            ctx.SummaryText.AppendLine("## Arrived");

            var arrivalTime = ctx.Command.ArrivalDate!.Value.ToDateTime(ctx.Command.ArrivalTime!.Value);
            ctx.SummaryText.AppendLine($"* Arrival Time: {arrivalTime.ToString("f")}");

            // schedule check work status for 4pm on the arrival date
            TimeOnly time = TimeOnly.FromTimeSpan(new TimeSpan(16, 0, 0));
            var rescheduleTime = ctx.Command.ArrivalDate!.Value.ToDateTime(time);
            self.ChangeStatus(ServiceBookingStatus.CheckWorkStatus, rescheduleTime);
        }
        public async Task NotArrived()
        {
            ctx.SummaryText.AppendLine("## Not Arrived");

            var reason = await ctx.DbContext.NonArrivalReasons.FindAsync(new object[] { self.CompanyCode, ctx.Command.NonArrivalReason! }, ctx.CancellationToken)
               ?? throw new InvalidOperationException("Failed to load non arrival reason.");

            ctx.SummaryText.AppendLine($"* Reason Code: {reason.Code}");
            ctx.SummaryText.AppendLine($"* Reason Text: {reason.Name}");

            if (self.ServiceLevel == ServiceLevel.Collection || self.ServiceLevel == ServiceLevel.Mobile)
                self.ChangeStatus(ServiceBookingStatus.RebookDriver, DateTime.Now);
            else
                self.ChangeStatus(ServiceBookingStatus.ChaseDriver, DateTime.Now);
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
