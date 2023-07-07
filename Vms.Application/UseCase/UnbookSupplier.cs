namespace Vms.Application.UseCase;

public class UnbookSupplier(VmsDbContext context)
{
    readonly VmsDbContext DBContext = context;

    public async Task UnbookAsync(Guid id, string reason, CancellationToken cancellationToken)
    {
        var serviceBooking = await DBContext.ServiceBookings.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load service booking.");

        serviceBooking.Unbook();
    }

}
