namespace Vms.Application.UseCase;

public class CreateCompany(VmsDbContext dbContext)
{
    readonly VmsDbContext DbContext = dbContext;

    public Company Create(CreateCompanyRequest request)
    {
        var company = new Company(request.Code, request.Name);
        DbContext.Add(company);

        //await DbContext.SaveChangesAsync(cancellationToken);

        return company;
    }
}


public record CreateCompanyRequest(string Code, string Name);
