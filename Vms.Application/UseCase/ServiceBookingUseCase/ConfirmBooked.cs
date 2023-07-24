using System.Text;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
using Vms.Web.Shared;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface IConfirmBooked
{
    Task ConfirmAsync(Guid id, TaskConfirmBookedCommand command, CancellationToken cancellationToken);
}

public class ConfirmBooked(VmsDbContext dbContext, IUserProvider userProvider) : IConfirmBooked
{
    readonly VmsDbContext DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    readonly IUserProvider UserProvider = userProvider;
    readonly StringBuilder SummaryText = new(); 
    ServiceBookingRole? ServiceBooking;

    public async Task ConfirmAsync(Guid id, TaskConfirmBookedCommand command, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        SummaryText.AppendLine("# Confirm Booked");

        switch (command.Result)
        {
            case TaskConfirmBookedCommand.TaskResult.Confirmed:
                ServiceBooking.Confirm();
                break;
            case TaskConfirmBookedCommand.TaskResult.Refused:
                ServiceBooking.Refused(); break;
            case TaskConfirmBookedCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule(Helper.CombineDateAndTime(command.RescheduleDate!.Value, command.RescheduleTime!.Value), command.RescheduleReason!);
                break;
        }

        DbContext.ActivityLog.Add(new ActivityLog(id, SummaryText.ToString(), UserProvider.UserId, UserProvider.UserName));
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
