using Vms.Web.Shared;

namespace Vms.Application.UseCase;

public class AddDriverToVehicle
{
    readonly VmsDbContext DbContext;
    VehicleRole? Vehicle;

    public AddDriverToVehicle(VmsDbContext dbContext)
        => DbContext = dbContext;

    public async Task AddAsync(Guid id, AddDriverToVehicleCommand command, CancellationToken cancellationToken = default)
    {
        Vehicle = new(await DbContext.Vehicles.FindAsync(id, cancellationToken)
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