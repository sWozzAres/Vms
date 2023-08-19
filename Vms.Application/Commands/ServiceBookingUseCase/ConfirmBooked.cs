using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public class ConfirmBooked(
    VmsDbContext dbContext,
    IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<ConfirmBooked> logger,
    ITimeService timeService) : ServiceBookingTaskBase(dbContext, activityLog)
{
    readonly ITimeService TimeService = timeService;
    ServiceBookingRole? ServiceBooking;
    TaskConfirmBookedCommand Command = null!;

    public async Task ConfirmAsync(Guid serviceBookingId, TaskConfirmBookedCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Confirming booked for service booking: {servicebookingid}, command: {@taskconfirmbookedcommand}.", serviceBookingId, command);

        Command = command;
        ServiceBooking = new(await Load(serviceBookingId, cancellationToken), this);

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
                await ServiceBooking.Reschedule(Command.RescheduleReason!,
                    Command.RescheduleDate!.Value.ToDateTime(Command.RescheduleTime!.Value));
                break;
        }

        if (!string.IsNullOrEmpty(Command.Callee))
            SummaryText.AppendLine($"* Callee: {Command.Callee}");

        await LogActivity();
        taskLogger.Log(Id, nameof(ConfirmBooked), Command);
    }

    class ServiceBookingRole(ServiceBooking self, ConfirmBooked ctx) : ServiceBookingRoleBase<ConfirmBooked>(self, ctx)
    {
        public void Confirm()
        {
            if (!Self.BookedDate.HasValue)
                throw new VmsDomainException("Service Booking has no booked date.");

            Ctx.SummaryText.AppendLine("## Confirmed");

            // schedule check arrival for 9am on the booking date
            var rescheduleTime = Self.BookedDate.Value.ToDateTime(TimeOnly.FromTimeSpan(new TimeSpan(9, 0, 0)));
            Self.ChangeStatus(ServiceBookingStatus.CheckArrival, rescheduleTime);
        }

        public async Task Refused()
        {
            Ctx.SummaryText.AppendLine("## Refused");

            var reason = await Ctx.DbContext.ConfirmBookedRefusalReasons.AsNoTracking()
                .SingleAsync(r => r.CompanyCode == Self.CompanyCode && r.Code == Ctx.Command.RefusalReason!, Ctx.CancellationToken);

            Ctx.SummaryText.AppendLine($"* Reason Code: {reason.Code}");
            Ctx.SummaryText.AppendLine($"* Reason Text: {reason.Name}");

            Self.Unbook();
            Self.Unassign();
            Self.ChangeStatus(ServiceBookingStatus.Assign, Ctx.TimeService.Now);

            //TODO
        }
    }
}