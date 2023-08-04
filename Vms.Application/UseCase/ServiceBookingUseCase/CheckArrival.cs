using System.Text;
using Vms.Application.Services;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Web.Shared;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface ICheckArrival
{
    Task CheckAsync(Guid id, TaskCheckArrivalCommand command, CancellationToken cancellationToken);
}

public class CheckArrival(VmsDbContext dbContext, IActivityLogger activityLog, ITaskLogger taskLogger) : ICheckArrival
{
    readonly VmsDbContext DbContext = dbContext;
    readonly StringBuilder SummaryText = new();

    ServiceBookingRole? ServiceBooking;
    Guid Id;
    TaskCheckArrivalCommand Command = null!;
    CancellationToken CancellationToken;

    public async Task CheckAsync(Guid id, TaskCheckArrivalCommand command, CancellationToken cancellationToken)
    {
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

        await activityLog.AddAsync(Id, SummaryText, CancellationToken);
        taskLogger.Log(Id, "Check Arrival", Command);
    }

    class ServiceBookingRole(ServiceBooking self, CheckArrival ctx)
    {
        public void Arrived()
        {
            ctx.SummaryText.AppendLine("## Arrived");

            // schedule check work status for 4pm on the arrival date
            TimeOnly time = TimeOnly.FromTimeSpan(new TimeSpan(9, 0, 0));
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
