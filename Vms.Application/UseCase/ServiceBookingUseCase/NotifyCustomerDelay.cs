﻿using System.Runtime.InteropServices.Marshalling;
using System.Text;
using Vms.Application.Services;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Services;
using Vms.Web.Shared;

namespace Vms.Application.UseCase.ServiceBookingUseCase;

public interface INotifyCustomerDelay
{
    Task NotifyAsync(Guid id, TaskNotifyCustomerDelayCommand command, CancellationToken cancellationToken);
}

public class NotifyCustomerDelay(VmsDbContext context, IActivityLogger activityLog, ITaskLogger taskLogger) : INotifyCustomerDelay
{
    readonly VmsDbContext DbContext = context;
    readonly IActivityLogger ActivityLog = activityLog;
    readonly ITaskLogger TaskLogger = taskLogger;
    readonly StringBuilder SummaryText = new();
    
    ServiceBookingRole? ServiceBooking;
    Guid Id;
    TaskNotifyCustomerDelayCommand Command = null!;
    CancellationToken CancellationToken;

    public async Task NotifyAsync(Guid id, TaskNotifyCustomerDelayCommand command, CancellationToken cancellationToken)
    {
        Id = id;
        Command = command ?? throw new ArgumentNullException(nameof(command));
        CancellationToken = cancellationToken;

        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { Id }, CancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        SummaryText.AppendLine("# Notify Customer Delay");

        switch (command.Result)
        {
            case TaskNotifyCustomerDelayCommand.TaskResult.Notified:
                ServiceBooking.CustomerNotified();
                break;
            case TaskNotifyCustomerDelayCommand.TaskResult.Rescheduled:
                ServiceBooking.Reschedule();
                break;
        }

        await ActivityLog.AddAsync(Id, SummaryText, CancellationToken);
        TaskLogger.Log(Id, "Notify Customer Delay", Command);
    }

    class ServiceBookingRole(ServiceBooking self, NotifyCustomerDelay ctx)
    {
        public void CustomerNotified()
        {
            ctx.SummaryText.AppendLine("## Customer Notified");
            self.ChangeStatus(ServiceBookingStatus.CheckWorkStatus, self.EstimatedCompletion);
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
