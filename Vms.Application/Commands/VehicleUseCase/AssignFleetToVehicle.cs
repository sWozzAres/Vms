﻿namespace Vms.Application.Commands.VehicleUseCase;

public class AssignFleetToVehicle(VmsDbContext context)
{
    readonly VmsDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    public async Task AssignAsync(Guid id, AssignFleetToVehicleCommand command, CancellationToken cancellationToken)
    {
        Assign(await _context.Vehicles.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new InvalidOperationException("Failed to load vehicle."), command.FleetCode);
    }

    public static void Assign(Vehicle v, string code)
    {
        v.AssignToFleet(code);
    }
}
