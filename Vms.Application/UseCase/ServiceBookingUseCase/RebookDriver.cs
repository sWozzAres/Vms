using System.Text;
using Vms.Application.Services;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
using Vms.Web.Shared;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface IRebookDriver
{
    Task RebookAsync(Guid id, TaskRebookDriverCommand command, CancellationToken cancellationToken);
}

public class RebookDriver(VmsDbContext dbContext, IActivityLogger activityLog, ITaskLogger taskLogger) : IRebookDriver
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IActivityLogger ActivityLog = activityLog;
    readonly ITaskLogger TaskLogger = taskLogger;
    readonly StringBuilder SummaryText = new();
    
    ServiceBookingRole? ServiceBooking;
    Guid Id;
    TaskRebookDriverCommand Command = null!;
    CancellationToken CancellationToken;

    public async Task RebookAsync(Guid id, TaskRebookDriverCommand command, CancellationToken cancellationToken)
    {
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

        await ActivityLog.AddAsync(Id, SummaryText, CancellationToken);
        TaskLogger.Log(Id, "Rebook Driver", Command);
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
            (await MotEvents()).ForEach(e => e.ServiceBookingId = null);

            self.ChangeStatus(ServiceBookingStatus.Cancelled);

            async Task<List<MotEvent>> MotEvents()
                => await ctx.DbContext.MotEvents.Where(e => e.ServiceBookingId == self.Id).ToListAsync(ctx.CancellationToken);
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
