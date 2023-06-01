using Vms.Domain.Entity;
using Vms.Domain.Exceptions;
using Vms.Domain.Infrastructure;

namespace Vms.Domain.UseCase;

public class CreateNetwork
{
    readonly VmsDbContext DbContext;
    CompanyRole? Company;
    public CreateNetwork(VmsDbContext dbContext)
       => DbContext = dbContext;

    public async Task<Network> CreateAsync(CreateNetworkRequest request, CancellationToken cancellationToken = default)
    {
        Company = new(await DbContext.Companies.FindAsync(request.CompanyCode)
            ?? throw new VmsDomainException("Company not found."), this);

        return await Company.CreateNetworkAsync(request.Code, request.Name, cancellationToken);
    }

    public class CompanyRole(Company self, CreateNetwork context)
    {
        public async Task<Network> CreateNetworkAsync(string code, string name, CancellationToken cancellationToken)
        {
            var network = new Network(self.Code, code, name);
            await context.DbContext.AddAsync(network, cancellationToken);
            return network;
        }
    }
}

public record CreateNetworkRequest(string CompanyCode, string Code, string Name);