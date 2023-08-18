using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface IConfirmBooked
{
    Task ConfirmAsync(Guid serviceBookingId, TaskConfirmBookedCommand command, CancellationToken cancellationToken);
}

public class ConfirmBooked(VmsDbContext dbContext, IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<ConfirmBooked> logger) : IConfirmBooked
{
    readonly VmsDbContext DbContext = dbContext;
    readonly StringBuilder SummaryText = new();

    ServiceBookingRole? ServiceBooking;
    Guid Id;
    TaskConfirmBookedCommand Command = null!;
    CancellationToken CancellationToken;

    public async Task ConfirmAsync(Guid serviceBookingId, TaskConfirmBookedCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Confirming booked for service booking: {servicebookingid}, command: {@taskconfirmbookedcommand}.", serviceBookingId, command);

        Id = serviceBookingId;
        Command = command ?? throw new ArgumentNullException(nameof(command));
        CancellationToken = cancellationToken;

        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { Id }, CancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        SummaryText.AppendLine("# Confirm Booked");

        switch (Command.Result)
        {
            case TaskConfirmBookedCommand.TaskResult.Confirmed:
                ServiceBooking.Confirm();
                break;
            case TaskConfirmBookedCommand.TaskResult.Refused:
                await ServiceBooking.Refused();
                break;
            case TaskConfirmBookedCommand.TaskResult.Rescheduled:
                await ServiceBooking.Reschedule();
                break;
        }

        if (!string.IsNullOrEmpty(Command.Callee))
            SummaryText.AppendLine($"* Callee: {Command.Callee}");

        _ = await activityLog.AddAsync(Id, SummaryText, CancellationToken);
        taskLogger.Log(Id, nameof(ConfirmBooked), Command);
    }

    class ServiceBookingRole(ServiceBooking self, ConfirmBooked ctx)
    {
        public void Confirm()
        {
            if (self.BookedDate is null)
                throw new VmsDomainException("Service Booking has no booked date.");

            ctx.SummaryText.AppendLine("## Confirmed");

            // schedule check arrival for 9am on the booking date
            TimeOnly time = TimeOnly.FromTimeSpan(new TimeSpan(9, 0, 0));
            var rescheduleTime = self.BookedDate.Value.ToDateTime(time);
            self.ChangeStatus(ServiceBookingStatus.CheckArrival, rescheduleTime);
        }

        public async Task Refused()
        {
            ctx.SummaryText.AppendLine("## Refused");
            var reason = await ctx.DbContext.ConfirmBookedRefusalReasons.FindAsync(new object[] { self.CompanyCode, ctx.Command.RefusalReason! }, ctx.CancellationToken)
                ?? throw new InvalidOperationException("Failed to load confirm booked refusal reason.");

            ctx.SummaryText.AppendLine($"* Reason Code: {reason.Code}");
            ctx.SummaryText.AppendLine($"* Reason Text: {reason.Name}");

            self.Unbook();
            self.Unassign();

            //TODO
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
