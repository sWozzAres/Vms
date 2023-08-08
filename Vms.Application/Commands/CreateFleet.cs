namespace Vms.Application.Commands;

public class CreateFleet(VmsDbContext dbContext, ISearchManager searchManager)
{
    readonly VmsDbContext DbContext = dbContext;
    CompanyRole? Company;

    public async Task<Fleet> CreateAsync(CreateFleetRequest request, CancellationToken cancellationToken = default)
    {
        Company = new(await DbContext.Companies.FindAsync(new object[] { request.CompanyCode }, cancellationToken)
            ?? throw new VmsDomainException("Company not found."), this);

        var fleet = Company.CreateFleet(request.Code, request.Name);

        searchManager.Add(fleet.CompanyCode, fleet.Code, EntityKind.Fleet, fleet.Name,
            string.Join(" ", fleet.Code, fleet.Name));


        return fleet;
    }

    class CompanyRole(Company self, CreateFleet context)
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