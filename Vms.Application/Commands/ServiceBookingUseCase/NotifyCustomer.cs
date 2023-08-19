using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public class NotifyCustomer(
    VmsDbContext dbContext,
    IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<NotifyCustomer> logger) : ServiceBookingTaskBase(dbContext, activityLog)
{
    ServiceBookingRole? ServiceBooking;
    TaskNotifyCustomerCommand Command = null!;

    public async Task NotifyAsync(Guid serviceBookingId, TaskNotifyCustomerCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Notifying customer for service booking: {servicebookingid}, command: {@tasknotifycustomercommand}.", serviceBookingId, command);

        Command = command;
        ServiceBooking = new(await Load(serviceBookingId, cancellationToken), this);

        SummaryText.AppendLine("# Notify Customer");

        switch (command.Result)
        {
            case TaskNotifyCustomerCommand.TaskResult.Notified:
                ServiceBooking.Notify();
                break;
            case TaskNotifyCustomerCommand.TaskResult.Rescheduled:
                await ServiceBooking.Reschedule(Command.RescheduleReason!,
                    Command.RescheduleDate!.Value.ToDateTime(Command.RescheduleTime!.Value));
                break;
        }

        if (!string.IsNullOrEmpty(Command.Callee))
            SummaryText.AppendLine($"* Callee: {Command.Callee}");

        await LogActivity();
        taskLogger.Log(Id, nameof(NotifyCustomer), Command);
    }

    class ServiceBookingRole(ServiceBooking self, NotifyCustomer ctx) : ServiceBookingRoleBase<NotifyCustomer>(self, ctx)
    {
        public void Notify()
        {
            Ctx.SummaryText.AppendLine("## Notified");
            Self.ChangeStatus(ServiceBookingStatus.Complete);
        }
    }
}
