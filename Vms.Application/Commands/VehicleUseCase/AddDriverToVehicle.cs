namespace Vms.Application.Commands.VehicleUseCase;

public class AddDriverToVehicle(VmsDbContext dbContext)
{
    readonly VmsDbContext DbContext = dbContext;
    VehicleRole? Vehicle;

    public async Task AddAsync(Guid id, AddDriverToVehicleCommand command, CancellationToken cancellationToken = default)
    {
        Vehicle = new(await DbContext.Vehicles.FindAsync(new object[] { id }, cancellationToken)
            ?? throw new VmsDomainException("Vehicle not found."), this);

        Vehicle.AddDriver(command.DriverId);

        //await DbContext.SaveChangesAsync(cancellationToken);
    }

    class VehicleRole(Vehicle self, AddDriverToVehicle context)
    {
        public void AddDriver(Guid driverId)
        {
            var dv = new DriverVehicle(self.CompanyCode, driverId, self.Id);
            context.DbContext.DriverVehicles.Add(dv);
        }
    }
}