using System.Text;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
using Vms.Web.Shared;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface ICheckArrival
{
    Task CheckAsync(Guid id, TaskCheckArrivalCommand command, CancellationToken cancellationToken);
}

public class CheckArrival(VmsDbContext dbContext, IUserProvider userProvider) : ICheckArrival
{
    readonly VmsDbContext DbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    readonly IUserProvider UserProvider = userProvider;
    readonly StringBuilder SummaryText = new(); 
    ServiceBookingRole? ServiceBooking;

    public async Task CheckAsync(Guid id, TaskCheckArrivalCommand command, CancellationToken cancellationToken)
    {
        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        SummaryText.AppendLine("# Check Arrival");

        switch (command.Result)
        {
            case TaskCheckArrivalCommand.TaskResult.Arrived:
                ServiceBooking.Arrived();
                break;
            case TaskCheckArrivalCommand.TaskResult.NotArrived:
                ServiceBooking.NotArrived();
                break;
            case TaskCheckArrivalCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule(Helper.CombineDateAndTime(command.RescheduleDate!.Value, command.RescheduleTime!.Value), command.RescheduleReason!);
                break;
        }

        DbContext.ActivityLog.Add(new ActivityLog(id, SummaryText.ToString(), UserProvider.UserId, UserProvider.UserName));
    }

    class ServiceBookingRole(ServiceBooking self, CheckArrival ctx)
    {
        public void Arrived()
        {
            ctx.SummaryText.AppendLine("## Arrived");
            self.ChangeStatus(ServiceBookingStatus.CheckWorkStatus);
        }
        public void NotArrived()
        {
            ctx.SummaryText.AppendLine("## Not Arrived");
            self.ChangeStatus(ServiceBookingStatus.ChaseDriver);
        }
        public void Reschedule(DateTime rescheduleTime, string reason)
        {
            ctx.SummaryText.AppendLine("## Rescheduled");
            ctx.SummaryText.AppendLine($"Rescheduled for {rescheduleTime.ToString("f")} because '{reason}'.");
            self.RescheduleTime = rescheduleTime;
        }
    }
}
