namespace Vms.Application.UseCase;

public class RemoveFleetFromVehicle(VmsDbContext context)
{
    readonly VmsDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task RemoveAsync(Guid id, CancellationToken cancellationToken)
    {
        Remove(await _context.Vehicles.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load vehicle."));
    }

    public static void Remove(Vehicle v)
    {
        v.RemoveFleet();
    }
}
