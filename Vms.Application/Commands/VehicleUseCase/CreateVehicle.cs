namespace Vms.Application.Commands.VehicleUseCase;

public interface ICreateVehicle
{
    Task<Vehicle> CreateAsync(CreateVehicleRequest request, CancellationToken cancellationToken = default);
}

public class CreateVehicle(VmsDbContext dbContext, ISearchManager searchManager) : ICreateVehicle
{
    readonly VmsDbContext DbContext = dbContext;
    CompanyRole? Company;

    public async Task<Vehicle> CreateAsync(CreateVehicleRequest request, CancellationToken cancellationToken = default)
    {
        Company = new(await DbContext.Companies.FindAsync(new object[] { request.CompanyCode }, cancellationToken)
            ?? throw new VmsDomainException("Company not found."), this);

        var vehicle = Company.CreateVehicle(request);

        searchManager.Add(vehicle.CompanyCode, vehicle.Id.ToString(), EntityKind.Vehicle, vehicle.Vrm, vehicle.Vrm);

        return vehicle;
    }

    class CompanyRole(Company self, CreateVehicle context)
    {
        public Vehicle CreateVehicle(CreateVehicleRequest request)
        {
            var vehicle = Vehicle.Create(self.Code,
                request.Vrm, request.Make, request.Model, request.DateFirstRegistered, request.MotDue, request.HomeLocation,
                request.CustomerCode, request.FleetCode);

            context.DbContext.Vehicles.Add(vehicle);

            return vehicle;
        }
    }
}

public record CreateVehicleRequest(string CompanyCode,
    string Vrm, string Make, string Model, DateOnly DateFirstRegistered, DateOnly? MotDue,
    Address HomeLocation,
    string? CustomerCode = null, string? FleetCode = null);