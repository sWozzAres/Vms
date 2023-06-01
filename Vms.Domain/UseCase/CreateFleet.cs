using Vms.Domain.Entity;
using Vms.Domain.Exceptions;
using Vms.Domain.Infrastructure;

namespace Vms.Domain.UseCase;

public class CreateFleet
{
    readonly VmsDbContext DbContext;
    CompanyRole? Company;
    public CreateFleet(VmsDbContext dbContext)
       => DbContext = dbContext;

    public async Task<Fleet> CreateAsync(CreateFleetRequest request, CancellationToken cancellationToken = default)
    {
        Company = new(await DbContext.Companies.FindAsync(request.CompanyCode)
            ?? throw new VmsDomainException("Company not found."), this);

        return await Company.CreateFleetAsync(request.Code, request.Name, cancellationToken);
    }

    public class CompanyRole(Company self, CreateFleet context)
    {
        public async Task<Fleet> CreateFleetAsync(string code, string name, CancellationToken cancellationToken)
        {
            var fleet = new Fleet(self.Code, code, name);
            await context.DbContext.AddAsync(fleet, cancellationToken);
            return fleet;
        }
    }
}

public record CreateFleetRequest(string  CompanyCode, string Code, string Name);