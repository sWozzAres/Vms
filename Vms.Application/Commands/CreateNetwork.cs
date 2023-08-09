using Microsoft.Extensions.Logging;

namespace Vms.Application.Commands;

public class CreateNetwork(VmsDbContext dbContext, ISearchManager searchManager, ILogger logger)
{
    readonly VmsDbContext DbContext = dbContext;
    CompanyRole? Company;

    public async Task<Network> CreateAsync(CreateNetworkRequest request, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating network {networkcode} {networkname}", request.Code, request.Name);

        Company = new(await DbContext.Companies.FindAsync(new object[] { request.CompanyCode }, cancellationToken)
            ?? throw new VmsDomainException("Company not found."), this);

        var network = Company.CreateNetwork(request.Code, request.Name);

        searchManager.Add(network.CompanyCode, network.Code, EntityKind.Network, network.Name,
            string.Join(" ", network.Code, network.Name));

        return network;
    }

    class CompanyRole(Company self, CreateNetwork context)
    {
        public Network CreateNetwork(string code, string name)
        {
            var network = new Network(self.Code, code, name);
            context.DbContext.Networks.Add(network);
            return network;
        }
    }
}

public record CreateNetworkRequest(string CompanyCode, string Code, string Name);