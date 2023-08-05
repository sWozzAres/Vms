﻿using System.Text;
using Vms.Application.Services;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Web.Shared;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface IChaseDriver
{
    Task ChaseAsync(Guid id, TaskChaseDriverCommand command, CancellationToken cancellationToken);
}

public class ChaseDriver(VmsDbContext dbContext, IActivityLogger activityLog, ITaskLogger taskLogger) : IChaseDriver
{
    readonly VmsDbContext DbContext = dbContext;
    readonly StringBuilder SummaryText = new();

    ServiceBookingRole? ServiceBooking;
    Guid Id;
    TaskChaseDriverCommand Command = null!;
    CancellationToken CancellationToken;

    public async Task ChaseAsync(Guid id, TaskChaseDriverCommand command, CancellationToken cancellationToken)
    {
        Id = id;
        Command = command ?? throw new ArgumentNullException(nameof(command));
        CancellationToken = cancellationToken;

        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { Id }, CancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        SummaryText.AppendLine("# Chase Driver");

        switch (command.Result)
        {
            case TaskChaseDriverCommand.TaskResult.StillGoing:
                ServiceBooking.StillGoing();
                break;
            case TaskChaseDriverCommand.TaskResult.NotGoing:
                ServiceBooking.NotGoing();
                break;
            case TaskChaseDriverCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule();
                break;
        }

        await activityLog.AddAsync(Id, SummaryText, CancellationToken);
        taskLogger.Log(Id, "Chase Driver", Command);
    }

    class ServiceBookingRole(ServiceBooking self, ChaseDriver ctx)
    {
        public void StillGoing()
        {
            ctx.SummaryText.AppendLine("## Still Going");
            // TODO log when they expect to arrive then reschedule for that
            self.ChangeStatus(ServiceBookingStatus.CheckArrival, DateTime.Now.AddMinutes(30));
        }
        public void NotGoing()
        {
            ctx.SummaryText.AppendLine("## Not Going");
            self.ChangeStatus(ServiceBookingStatus.RebookDriver, DateTime.Now);
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