using System.Text;
using Vms.Application.Services;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
using Vms.Web.Shared;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface IConfirmBooked
{
    Task ConfirmAsync(Guid id, TaskConfirmBookedCommand command, CancellationToken cancellationToken);
}

public class ConfirmBooked(VmsDbContext dbContext, IActivityLogger activityLog, ITaskLogger taskLogger) : IConfirmBooked
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IActivityLogger ActivityLog = activityLog;
    readonly ITaskLogger TaskLogger = taskLogger;
    readonly StringBuilder SummaryText = new(); 
    ServiceBookingRole? ServiceBooking;

    public async Task ConfirmAsync(Guid id, TaskConfirmBookedCommand command, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        SummaryText.AppendLine("# Confirm Booked");

        switch (command.Result)
        {
            case TaskConfirmBookedCommand.TaskResult.Confirmed:
                ServiceBooking.Confirm();
                break;
            case TaskConfirmBookedCommand.TaskResult.Refused:
                ServiceBooking.Refused(); 
                break;
            case TaskConfirmBookedCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule(Helper.CombineDateAndTime(command.RescheduleDate!.Value, command.RescheduleTime!.Value), command.RescheduleReason!);
                break;
        }

        await ActivityLog.LogAsync(id, SummaryText, cancellationToken);
        TaskLogger.Log(id, "Confirm Booked", command);
    }

    class ServiceBookingRole(ServiceBooking self, ConfirmBooked ctx)
    {
        public void Confirm()
        {
            ctx.SummaryText.AppendLine("## Confirm");
            self.ChangeStatus(ServiceBookingStatus.CheckArrival);
        }

        public void Refused()
        {
            ctx.SummaryText.AppendLine("## Refused");
            self.Unbook();
            self.Unassign();

            //TODO
        }

        public void Reschedule(DateTime rescheduleTime, string reason)
        {
            ctx.SummaryText.AppendLine("## Rescheduled");
            ctx.SummaryText.AppendLine($"Rescheduled for {rescheduleTime.ToString("f")} because '{reason}'.");
            self.RescheduleTime = rescheduleTime;
        }
    }
}
