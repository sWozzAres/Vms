using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Vms.Application.Commands.ServiceBookingUseCase;

public interface IUnbookSupplier
{
    Task UnbookAsync(Guid id, TaskUnbookSupplierCommand command, CancellationToken cancellationToken);
}

public class UnbookSupplier(VmsDbContext dbContext, IActivityLogger<VmsDbContext> activityLog,
    ITaskLogger<VmsDbContext> taskLogger,
    ILogger<UnbookSupplier> logger) : IUnbookSupplier
{
    readonly VmsDbContext DbContext = dbContext;
    readonly StringBuilder SummaryText = new();

    public async Task UnbookAsync(Guid id, TaskUnbookSupplierCommand command, CancellationToken cancellationToken)
    {
        logger.LogInformation("Unbooking supplier for service booking: {servicebookingid}, command: {@taskunbooksuppliercommand}.", id, command);

        var serviceBooking = await DbContext.ServiceBookings.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking.");

        SummaryText.AppendLine("# Unbook Supplier");
        SummaryText.AppendLine($"* Reason: {command.Reason}");

        serviceBooking.Unbook();

        _ = await activityLog.AddAsync(id, SummaryText, cancellationToken);
        taskLogger.Log(id, nameof(UnbookSupplier), command);
    }
}
