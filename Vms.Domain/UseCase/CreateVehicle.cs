using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        Company = new(await DbContext.Companies.FindAsync(request.CompanyId)
            ?? throw new VmsDomainException("Company not found."), this);

        var vehicle = await Company.CreateVehicleAsync(request, cancellationToken);
        //await DbContext.SaveChangesAsync(cancellationToken);
        return vehicle;
    }

    public class CompanyRole(Company self, CreateVehicle context)
    {
        public async Task<Vehicle> CreateVehicleAsync(CreateVehicleRequest request, CancellationToken cancellationToken)
        {
            var vehicle = new Vehicle(self.Code, 
                request.Vrm, request.Make, request.Model, request.DateFirstRegistered, request.MotDue);
            
            await context.DbContext.AddAsync(vehicle, cancellationToken);
            
            return vehicle;
        }
    }
}

public record CreateVehicleRequest(int CompanyId, string Vrm, string Make, string Model, DateOnly DateFirstRegistered, DateOnly MotDue);