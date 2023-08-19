using Vms.Domain.ServiceBookingProcess;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface IUnbookSupplier
{
    Task UnbookAsync(Guid serviceBookingId, TaskUnbookSupplierCommand command, CancellationToken cancellationToken);
}

public class UnbookSupplier(VmsDbContext dbContext, IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<UnbookSupplier> logger) : IUnbookSupplier
{
    readonly VmsDbContext DbContext = dbContext;
    readonly StringBuilder SummaryText = new();

    public async Task UnbookAsync(Guid serviceBookingId, TaskUnbookSupplierCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Unbooking supplier for service booking: {servicebookingid}, command: {@taskunbooksuppliercommand}.", serviceBookingId, command);

        var serviceBooking = await DbContext.ServiceBookings.FindAsync(new object[] { serviceBookingId }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking.");

        SummaryText.AppendLine("# Unbook Supplier");
        SummaryText.AppendLine($"* Reason: {command.Reason}");

        serviceBooking.Unbook();

        _ = await activityLog.AddAsync(serviceBookingId, nameof(ServiceBooking), serviceBooking.Ref, SummaryText, cancellationToken);
        taskLogger.Log(serviceBookingId, nameof(UnbookSupplier), command);
    }
}
