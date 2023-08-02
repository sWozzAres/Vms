namespace Vms.Application.UseCase.VehicleUseCase;

public class ChangeVrm(VmsDbContext context)
{
    readonly VmsDbContext DbContext = context;

    public async Task ChangeTo(ChangeVrmRequest request, CancellationToken cancellationToken = default)
    {
        var vehicle = await DbContext.Vehicles.FindAsync(new object[] { request.VehicleId }, cancellationToken)
            ?? throw new VmsDomainException("Vehicle not found.");
        
        vehicle.Vrm = request.NewVrm;

        //Vehicle = new(await DbContext.Vehicles.FindAsync(request.vehicleId, cancellationToken)
        //    ?? throw new VmsDomainException("Vehicle not found."), this);

        //Vehicle.ChangeVrm(request.newVrm);

    }

    //class VehicleRole(Vehicle self, ChangeVrm context)
    //{
    //    public void ChangeVrm(string newVrm) => self.Vrm = newVrm;
    //}
}

public record ChangeVrmRequest(Guid VehicleId, string NewVrm);
