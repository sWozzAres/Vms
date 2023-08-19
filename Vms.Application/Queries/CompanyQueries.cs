namespace Vms.Application.Queries;

public class CompanyQueries(VmsDbContext context
    //IUserProvider userProvider
    )
{
    public async Task<(int TotalCount, List<CompanyListDto> Result)> GetCompanies(
        CompanyListOptions list, int start, int take, CancellationToken cancellationToken)
    {
        var companies = context.Companies.AsNoTracking()
            .AsQueryable();

        switch (list)
        {
            case CompanyListOptions.All:
                break;
            default:
                throw new NotSupportedException($"Unknown list option '{list}'.");
        }

        int totalCount = await companies.CountAsync(cancellationToken);

        var result = await companies
            .OrderBy(c => c.Code)
            .Skip(start)
            .Take(take)

            .Select(x => new CompanyListDto(x.Code, x.Name))
            .ToListAsync(cancellationToken);

        return (totalCount, result);

    }
}
