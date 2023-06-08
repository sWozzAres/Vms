namespace Vms.Application.UseCase;

public class CreateFleet
{
    readonly VmsDbContext DbContext;
    CompanyRole? Company;
    public CreateFleet(VmsDbContext dbContext)
       => DbContext = dbContext;

    public async Task<Fleet> CreateAsync(CreateFleetRequest request, CancellationToken cancellationToken = default)
    {
        Company = new(await DbContext.Companies.FindAsync(request.CompanyCode, cancellationToken)
            ?? throw new VmsDomainException("Company not found."), this);

        return Company.CreateFleet(request.Code, request.Name);
    }

    public class CompanyRole(Company self, CreateFleet context)
    {
        public Fleet CreateFleet(string code, string name)
        {
            var fleet = new Fleet(self.Code, code, name);
            context.DbContext.Fleets.Add(fleet);
            return fleet;
        }
    }
}

public record CreateFleetRequest(string CompanyCode, string Code, string Name);