namespace Vms.Application.UseCase;

public class ChangeVrm
{
    readonly VmsDbContext DbContext;
    //VehicleRole? Vehicle;

    public ChangeVrm(VmsDbContext context) => DbContext = context;

    public async Task ChangeTo(ChangeVrmRequest request, CancellationToken cancellationToken = default)
    {
        var vehicle = await DbContext.Vehicles.FindAsync(request.vehicleId, cancellationToken)
            ?? throw new VmsDomainException("Vehicle not found.");
        
        vehicle.Vrm = request.newVrm;

        //Vehicle = new(await DbContext.Vehicles.FindAsync(request.vehicleId, cancellationToken)
        //    ?? throw new VmsDomainException("Vehicle not found."), this);

        //Vehicle.ChangeVrm(request.newVrm);

    }

    //class VehicleRole(Vehicle self, ChangeVrm context)
    //{
    //    public void ChangeVrm(string newVrm) => self.Vrm = newVrm;
    //}
}

public record ChangeVrmRequest(Guid vehicleId, string newVrm);
