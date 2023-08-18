﻿using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface INotifyCustomerDelay
{
    Task NotifyAsync(Guid serviceBookingId, TaskNotifyCustomerDelayCommand command, CancellationToken cancellationToken);
}

public class NotifyCustomerDelay(VmsDbContext context, IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<NotifyCustomerDelay> logger) : INotifyCustomerDelay
{
    readonly VmsDbContext DbContext = context;
    readonly StringBuilder SummaryText = new();

    ServiceBookingRole? ServiceBooking;
    Guid Id;
    TaskNotifyCustomerDelayCommand Command = null!;
    CancellationToken CancellationToken;

    public async Task NotifyAsync(Guid serviceBookingId, TaskNotifyCustomerDelayCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Notifying customer of delay for service booking: {servicebookingid}, command: {@tasknotifycustomerdelaycommand}.", serviceBookingId, command);

        Id = serviceBookingId;
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
                await ServiceBooking.Reschedule();
                break;
        }

        _ = await activityLog.AddAsync(Id, SummaryText, CancellationToken);
        taskLogger.Log(Id, nameof(NotifyCustomerDelay), Command);
    }

    class ServiceBookingRole(ServiceBooking self, NotifyCustomerDelay ctx)
    {
        public void CustomerNotified()
        {
            ctx.SummaryText.AppendLine("## Customer Notified");
            self.ChangeStatus(ServiceBookingStatus.CheckWorkStatus, self.EstimatedCompletion);
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
