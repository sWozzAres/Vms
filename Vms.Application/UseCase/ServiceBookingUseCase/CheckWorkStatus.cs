using System.Text;
using Vms.Application.Services;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
using Vms.Web.Shared;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface ICheckWorkStatus
{
    Task CheckAsync(Guid id, TaskCheckWorkStatusCommand command, CancellationToken cancellationToken);
}

public class CheckWorkStatus(VmsDbContext dbContext, IActivityLogger activityLog, ITaskLogger taskLogger) : ICheckWorkStatus
{
    readonly VmsDbContext DbContext = dbContext;
    readonly IActivityLogger ActivityLog = activityLog;
    readonly ITaskLogger TaskLogger = taskLogger;
    readonly StringBuilder SummaryText = new(); 
    
    ServiceBookingRole? ServiceBooking;
    Guid Id;
    TaskCheckWorkStatusCommand Command = null!;
    CancellationToken CancellationToken;

    public async Task CheckAsync(Guid id, TaskCheckWorkStatusCommand command, CancellationToken cancellationToken)
    {
        Id = id;
        Command = command ?? throw new ArgumentNullException(nameof(command));  
        CancellationToken = cancellationToken;

        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { Id }, CancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        SummaryText.AppendLine("# Check Work Status");

        switch (command.Result)
        {
            case TaskCheckWorkStatusCommand.TaskResult.Complete:
                await ServiceBooking.Complete();
                break;
            case TaskCheckWorkStatusCommand.TaskResult.NotComplete:
                ServiceBooking.NotComplete();
                break;
            case TaskCheckWorkStatusCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule();
                break;
        }

        await ActivityLog.AddAsync(Id, SummaryText, CancellationToken);
        TaskLogger.Log(Id, "Check Work Status", Command);
    }

    class ServiceBookingRole(ServiceBooking self, CheckWorkStatus ctx)
    {
        public async Task Complete()
        {
            ctx.SummaryText.AppendLine("## Complete");

            var motEvent = await ctx.DbContext.MotEvents.SingleOrDefaultAsync(e => e.ServiceBookingId == self.Id && e.IsCurrent);

            if (motEvent is not null)
            {
                motEvent.IsCurrent = false;

                // next MOT is 1 year from either last MOT date or this MOT completion date, whichever is later
                var nextMotDate = ((ctx.Command.CompletionDate!.Value > motEvent.Due)
                    ? ctx.Command.CompletionDate!.Value
                    : motEvent.Due).AddYears(1);

                var nextMotEvent = new MotEvent(motEvent.CompanyCode, motEvent.VehicleId, nextMotDate, true);
                ctx.DbContext.MotEvents.Add(nextMotEvent);
            }

            self.ChangeStatus(ServiceBookingStatus.NotifyCustomer, DateTime.Now);

            
        }
        public void NotComplete()
        {
            var nextChase = ctx.Command.NextChaseDate!.Value.ToDateTime(ctx.Command.NextChaseTime!.Value);
            ctx.SummaryText.AppendLine("## Not Complete");
            self.EstimatedCompletion = nextChase;
            self.ChangeStatus(ServiceBookingStatus.NotifyCustomerDelay, DateTime.Now);

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
