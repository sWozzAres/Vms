namespace Vms.Application.UseCase;

public class CreateNetwork
{
    readonly VmsDbContext DbContext;
    CompanyRole? Company;
    public CreateNetwork(VmsDbContext dbContext)
       => DbContext = dbContext;

    public async Task<Network> CreateAsync(CreateNetworkRequest request, CancellationToken cancellationToken = default)
    {
        Company = new(await DbContext.Companies.FindAsync(request.CompanyCode, cancellationToken)
            ?? throw new VmsDomainException("Company not found."), this);

        var network = Company.CreateNetwork(request.Code, request.Name);

        //await DbContext.SaveChangesAsync(cancellationToken);

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