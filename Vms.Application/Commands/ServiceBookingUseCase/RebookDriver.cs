using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface IRebookDriver
{
    Task RebookAsync(Guid serviceBookingId, TaskRebookDriverCommand command, CancellationToken cancellationToken);
}

public class RebookDriver(VmsDbContext dbContext, IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<RebookDriver> logger) : IRebookDriver
{
    readonly VmsDbContext DbContext = dbContext;
    readonly StringBuilder SummaryText = new();

    ServiceBookingRole? ServiceBooking;
    Guid Id;
    TaskRebookDriverCommand Command = null!;
    CancellationToken CancellationToken;

    public async Task RebookAsync(Guid serviceBookingId, TaskRebookDriverCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Rebooking with driver for service booking: {servicebookingid}, command: {@taskrebookdrivercommand}.", serviceBookingId, command);

        Id = serviceBookingId;
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
                await ServiceBooking.Reschedule();
                break;
        }

        _ = await activityLog.AddAsync(serviceBookingId, nameof(Domain.ServiceBookingProcess.ServiceBooking), ServiceBooking.Entity.Ref,
            SummaryText, CancellationToken);
        taskLogger.Log(Id, nameof(RebookDriver), Command);
    }

    class ServiceBookingRole(ServiceBooking self, RebookDriver ctx)
    {
        public ServiceBooking Entity => self;
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
            if (self.MotEventId is not null)
            {
                _ = await ctx.DbContext.MotEvents.FindAsync(new object[] { self.MotEventId }, ctx.CancellationToken)
                    ?? throw new InvalidOperationException("Failed to load Mot Event.");
                self.RemoveMotEvent();
            }

            self.ChangeStatus(ServiceBookingStatus.Cancelled);
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
