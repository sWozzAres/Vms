namespace Vms.Application.UseCase;

public class CreateCompany
{
    readonly VmsDbContext DbContext;

    public CreateCompany(VmsDbContext dbContext)
       => DbContext = dbContext;

    public Company Create(CreateCompanyRequest request)
    {
        var company = new Company(request.Code, request.Name);
        DbContext.Add(company);

        //await DbContext.SaveChangesAsync(cancellationToken);

        return company;
    }
}


public record CreateCompanyRequest(string Code, string Name);
