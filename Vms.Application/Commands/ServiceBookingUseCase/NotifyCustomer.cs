using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface INotifyCustomer
{
    Task NotifyAsync(Guid id, TaskNotifyCustomerCommand command, CancellationToken cancellationToken);
}

public class NotifyCustomer(VmsDbContext dbContext, IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<NotifyCustomer> logger) : INotifyCustomer
{
    readonly VmsDbContext DbContext = dbContext;
    readonly StringBuilder SummaryText = new();

    ServiceBookingRole? ServiceBooking;
    Guid Id;
    TaskNotifyCustomerCommand Command = null!;
    CancellationToken CancellationToken;

    public async Task NotifyAsync(Guid id, TaskNotifyCustomerCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Notifying customer for service booking: {servicebookingid}, command: {@tasknotifycustomercommand}.", id, command);

        Id = id;
        Command = command ?? throw new ArgumentNullException(nameof(command));
        CancellationToken = cancellationToken;

        ServiceBooking = new(await DbContext.ServiceBookings.FindAsync(new object[] { Id }, CancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking."), this);

        SummaryText.AppendLine("# Notify Customer");

        switch (command.Result)
        {
            case TaskNotifyCustomerCommand.TaskResult.Notified:
                ServiceBooking.Notify();
                break;
            case TaskNotifyCustomerCommand.TaskResult.Rescheduled:
                await ServiceBooking.Reschedule();
                break;
        }

        if (!string.IsNullOrEmpty(Command.Callee))
            SummaryText.AppendLine($"* Callee: {Command.Callee}");

        _ = await activityLog.AddAsync(Id, SummaryText, CancellationToken);
        taskLogger.Log(Id, nameof(NotifyCustomer), Command);
    }

    class ServiceBookingRole(ServiceBooking self, NotifyCustomer ctx)
    {
        public void Notify()
        {
            ctx.SummaryText.AppendLine("## Notified");
            self.ChangeStatus(ServiceBookingStatus.Complete);
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
