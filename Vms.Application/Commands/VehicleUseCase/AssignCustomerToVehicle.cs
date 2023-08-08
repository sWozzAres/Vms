namespace Vms.Application.Commands.VehicleUseCase;

public class AssignCustomerToVehicle(VmsDbContext context)
{
    readonly VmsDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task AssignAsync(Guid id, AssignCustomerToVehicleCommand command, CancellationToken cancellationToken)
    {
        Assign(await _context.Vehicles.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load vehicle."), command.CustomerCode);
    }

    public static void Assign(Vehicle v, string code)
    {
        v.AssignToCustomer(code);
    }
}
