namespace Vms.Application.UseCase;

public class AssignFleetToVehicle(VmsDbContext context)
{
    readonly VmsDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task AssignAsync(Guid id, string code, CancellationToken cancellationToken)
    {
        Assign(await _context.Vehicles.FindAsync(id, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load vehicle."), code);
    }

    public static void Assign(Vehicle v, string code)
    {
        v.AssignToFleet(code);
    }
}
