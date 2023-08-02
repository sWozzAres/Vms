namespace Vms.Application.UseCase;

public class CreateFleet(VmsDbContext dbContext)
{
    readonly VmsDbContext DbContext = dbContext;
    CompanyRole? Company;

    public async Task<Fleet> CreateAsync(CreateFleetRequest request, CancellationToken cancellationToken = default)
    {
        Company = new(await DbContext.Companies.FindAsync(new object[] { request.CompanyCode }, cancellationToken)
            ?? throw new VmsDomainException("Company not found."), this);

        var fleet = Company.CreateFleet(request.Code, request.Name);

        //await DbContext.SaveChangesAsync(cancellationToken); 
        
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