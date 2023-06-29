namespace Vms.Application.UseCase;

public class CreateVehicle
{
    readonly VmsDbContext DbContext;
    CompanyRole? Company;
    public CreateVehicle(VmsDbContext dbContext)
       => DbContext = dbContext;

    public async Task<Vehicle> CreateAsync(CreateVehicleRequest request, CancellationToken cancellationToken = default)
    {
        Company = new(await DbContext.Companies.FindAsync(request.CompanyCode, cancellationToken)
            ?? throw new VmsDomainException("Company not found."), this);

        return Company.CreateVehicle(request);
    }

    public class CompanyRole(Company self, CreateVehicle context)
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
    string Vrm, string Make, string Model, DateOnly DateFirstRegistered, DateOnly MotDue,
    Address HomeLocation,
    string? CustomerCode = null, string? FleetCode = null);