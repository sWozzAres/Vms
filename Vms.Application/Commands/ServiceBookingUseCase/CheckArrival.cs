using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface ICheckArrival
{
    Task CheckAsync(Guid id, TaskCheckArrivalCommand command, CancellationToken cancellationToken);
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

    public async Task CheckAsync(Guid id, TaskCheckArrivalCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Checking arrival for service booking: {servicebookingid}, command: {@taskcheckarrivalcommand}.", id, command);

        Id = id;
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
                ServiceBooking.NotArrived();
                break;
            case TaskCheckArrivalCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule();
                break;
        }

        _ = await activityLog.AddAsync(Id, SummaryText, CancellationToken);
        taskLogger.Log(Id, nameof(CheckArrival), Command);
    }

    class ServiceBookingRole(ServiceBooking self, CheckArrival ctx)
    {
        public void Arrived()
        {
            ctx.SummaryText.AppendLine("## Arrived");

            // schedule check work status for 4pm on the arrival date
            TimeOnly time = TimeOnly.FromTimeSpan(new TimeSpan(16, 0, 0));
            var rescheduleTime = ctx.Command.ArrivalDate!.Value.ToDateTime(time);
            self.ChangeStatus(ServiceBookingStatus.CheckWorkStatus, rescheduleTime);
        }
        public void NotArrived()
        {
            ctx.SummaryText.AppendLine("## Not Arrived");

            if (self.ServiceLevel == ServiceLevel.Collection || self.ServiceLevel == ServiceLevel.Mobile)
                self.ChangeStatus(ServiceBookingStatus.RebookDriver, DateTime.Now);
            else
                self.ChangeStatus(ServiceBookingStatus.ChaseDriver, DateTime.Now);
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
