namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface IUnbookSupplier
{
    Task UnbookAsync(Guid id, TaskUnbookSupplierCommand command, CancellationToken cancellationToken);
}

public class UnbookSupplier(VmsDbContext dbContext, IActivityLogger activityLog, ITaskLogger taskLogger) : IUnbookSupplier
{
    readonly VmsDbContext DbContext = dbContext;
    readonly StringBuilder SummaryText = new();

    public async Task UnbookAsync(Guid id, TaskUnbookSupplierCommand command, CancellationToken cancellationToken)
    {
        var serviceBooking = await DbContext.ServiceBookings.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking.");

        SummaryText.AppendLine("# Unbook Supplier");

        serviceBooking.Unbook();

        await activityLog.AddAsync(id, SummaryText, cancellationToken);
        taskLogger.Log(id, "Unbook Supplier", command);
    }
}
