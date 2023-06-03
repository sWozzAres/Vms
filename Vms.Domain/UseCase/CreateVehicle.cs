using NetTopologySuite.Geometries;
using Vms.Domain.Entity;
using Vms.Domain.Exceptions;
using Vms.Domain.Infrastructure;

namespace Vms.Domain.UseCase;

public class CreateVehicle
{
    readonly VmsDbContext DbContext;
    CompanyRole? Company;
    public CreateVehicle(VmsDbContext dbContext)
       => DbContext = dbContext;

    public async Task<Vehicle> CreateAsync(CreateVehicleRequest request, CancellationToken cancellationToken = default)
    {
        Company = new(await DbContext.Companies.FindAsync(request.CompanyCode)
            ?? throw new VmsDomainException("Company not found."), this);

        return await Company.CreateVehicleAsync(request, cancellationToken);
    }

    public class CompanyRole(Company self, CreateVehicle context)
    {
        public async Task<Vehicle> CreateVehicleAsync(CreateVehicleRequest request, CancellationToken cancellationToken)
        {
            var vehicle = new Vehicle(self.Code, 
                request.Vrm, request.Make, request.Model, request.DateFirstRegistered, request.MotDue, request.homeLocation);
            
            if (request.FleetCode is not null)
            {
                vehicle.Fleet = await context.DbContext.Fleets.FindAsync(new[] { self.Code, request.FleetCode }, cancellationToken)
                    ?? throw new VmsDomainException("Fleet not found.");
            }

            if (request.CustomerCode is not null)
            {
                vehicle.C = await context.DbContext.Customers.FindAsync(new[] { self.Code, request.CustomerCode }, cancellationToken)
                    ?? throw new VmsDomainException("Customer not found.");
            }

            await context.DbContext.AddAsync(vehicle, cancellationToken);
            
            return vehicle;
        }
    }
}

public record CreateVehicleRequest(string CompanyCode, string Vrm, string Make, string Model, DateOnly DateFirstRegistered, DateOnly MotDue,
    Point homeLocation,
    string? CustomerCode = null, string? FleetCode = null);