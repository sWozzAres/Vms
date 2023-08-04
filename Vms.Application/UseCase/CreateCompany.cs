using Vms.Application.Services;

namespace Vms.Application.UseCase;

public class CreateCompany(VmsDbContext dbContext, ISearchManager searchManager)
{
    readonly VmsDbContext DbContext = dbContext;

    public Company Create(CreateCompanyRequest request)
    {
        var company = new Company(request.Code, request.Name);
        DbContext.Add(company);

        searchManager.Add(company.Code, company.Code, EntityKind.Company, company.Name,
            string.Join(" ", company.Code, company.Name));

        return company;
    }
}


public record CreateCompanyRequest(string Code, string Name);
