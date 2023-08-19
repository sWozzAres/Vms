using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public class NotifyCustomerDelay(
    VmsDbContext dbContext,
    IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<NotifyCustomerDelay> logger) : ServiceBookingTaskBase(dbContext, activityLog)
{
    ServiceBookingRole? ServiceBooking;
    TaskNotifyCustomerDelayCommand Command = null!;

    public async Task NotifyAsync(Guid serviceBookingId, TaskNotifyCustomerDelayCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Notifying customer of delay for service booking: {servicebookingid}, command: {@tasknotifycustomerdelaycommand}.", serviceBookingId, command);

        Command = command;
        ServiceBooking = new(await Load(serviceBookingId, cancellationToken), this);

        SummaryText.AppendLine("# Notify Customer Delay");

        switch (command.Result)
        {
            case TaskNotifyCustomerDelayCommand.TaskResult.Notified:
                ServiceBooking.CustomerNotified();
                break;
            case TaskNotifyCustomerDelayCommand.TaskResult.Rescheduled:
                await ServiceBooking.Reschedule(Command.RescheduleReason!,
                    Command.RescheduleDate!.Value.ToDateTime(Command.RescheduleTime!.Value));
                break;
        }

        await LogActivity();
        taskLogger.Log(Id, nameof(NotifyCustomerDelay), Command);
    }

    class ServiceBookingRole(ServiceBooking self, NotifyCustomerDelay ctx) : ServiceBookingRoleBase<NotifyCustomerDelay>(self, ctx)
    {
        public void CustomerNotified()
        {
            Ctx.SummaryText.AppendLine("## Customer Notified");
            Self.ChangeStatus(ServiceBookingStatus.CheckWorkStatus, Self.EstimatedCompletion);
        }
    }
}
