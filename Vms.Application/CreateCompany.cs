using Vms.Domain.Entity;
using Vms.Domain.Infrastructure;

namespace Vms.Application.UseCase;

public class CreateCompany
{
    readonly VmsDbContext DbContext;

    public CreateCompany(VmsDbContext dbContext)
       => DbContext = dbContext;

    public async Task<Company> CreateAsync(CreateCompanyRequest request, CancellationToken cancellationToken = default)
    {
        var company = new Company(request.Code, request.Name);

        await DbContext.AddAsync(company);

        return company;
    }
}


public record CreateCompanyRequest(string Code, string Name);
