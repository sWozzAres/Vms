namespace Vms.Application.Commands.CompanyUseCase;

public class CreateCompany(VmsDbContext dbContext,
    ISearchManager searchManager,
    ILogger<CreateCompany> logger,
    ITimeService timeService)
{
    readonly VmsDbContext DbContext = dbContext;

    public Company Create(CreateCompanyRequest request)
    {
        logger.LogInformation("Creating company {companycode} {companyname}", request.Code, request.Name);

        var company = new Company(request.Code, request.Name, timeService.Now);
        DbContext.Add(company);

        searchManager.Add(company.Code, company.Code, EntityKind.Company, company.Name,
            string.Join(" ", company.Code, company.Name));

        return company;
    }
}


public record CreateCompanyRequest(string Code, string Name);
