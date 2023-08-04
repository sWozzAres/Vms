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
    Guid Id;
    TaskConfirmBookedCommand Command = null!;
    CancellationToken CancellationToken;

    public async Task ConfirmAsync(Guid id, TaskConfirmBookedCommand command, CancellationToken cancellationToken)
    {
        Id = id;
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
                ServiceBooking.Refused(); 
                break;
            case TaskConfirmBookedCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule();
                break;
        }

        await ActivityLog.AddAsync(Id, SummaryText, CancellationToken);
        TaskLogger.Log(Id, "Confirm Booked", Command);
    }

    class ServiceBookingRole(ServiceBooking self, ConfirmBooked ctx)
    {
        public void Confirm()
        {
            if (self.BookedDate is null)
                throw new VmsDomainException("Service Booking has no booked date.");

            ctx.SummaryText.AppendLine("## Confirm");

            // schedule check arrival for 9am on the booking date
            TimeOnly time = TimeOnly.FromTimeSpan(new TimeSpan(9, 0, 0));
            var rescheduleTime = self.BookedDate.Value.ToDateTime(time);
            self.ChangeStatus(ServiceBookingStatus.CheckArrival, rescheduleTime);
        }

        public void Refused()
        {
            ctx.SummaryText.AppendLine("## Refused");
            self.Unbook();
            self.Unassign();

            //TODO
        }
        
        public void Reschedule()
        {
            var rescheduleTime = Helper.CombineDateAndTime(ctx.Command.RescheduleDate!.Value, ctx.Command.RescheduleTime!.Value);
            ctx.SummaryText.AppendLine("## Rescheduled");
            ctx.SummaryText.AppendLine($"Rescheduled for {rescheduleTime.ToString("f")} because '{ctx.Command.RescheduleReason!}'.");
            self.RescheduleTime = rescheduleTime;
        }
    }
}
