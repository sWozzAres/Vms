﻿namespace Vms.Application.Commands.VehicleUseCase;

public class RemoveDriverFromVehicle(VmsDbContext dbContext)
{
    readonly VmsDbContext DbContext = dbContext;
    VehicleRole? Vehicle;

    public async Task<bool> RemoveAsync(Guid id, Guid driverId, CancellationToken cancellationToken = default)
    {
        // load vehicle - ensures we have access
        Vehicle = new(await DbContext.Vehicles.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new VmsDomainException("Vehicle not found."), this);

        if (!await Vehicle.RemoveDriverAsync(driverId, cancellationToken))
        {
            return false;
        }

        //await DbContext.SaveChangesAsync(cancellationToken);

        return true;
    }

    class VehicleRole(Vehicle self, RemoveDriverFromVehicle context)
    {
        public async Task<bool> RemoveDriverAsync(Guid driverId, CancellationToken cancellationToken)
        {
            var entry = await context.DbContext.DriverVehicles
                .FindAsync(new object[] { self.CompanyCode, driverId, self.Id }, cancellationToken);
            if (entry is null)
            {
                return false;
            }

            context.DbContext.DriverVehicles.Remove(entry);
            return true;
        }
    }
}