namespace Vms.Application.Commands.VehicleUseCase;

public class RemoveCustomerFromVehicle(VmsDbContext context)
{
    readonly VmsDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken)
    {
        Remove(await _context.Vehicles.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load vehicle."));
    }

    public static void Remove(Vehicle v)
    {
        v.RemoveCustomer();
    }
}
