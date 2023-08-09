namespace Vms.Application.Queries;

public interface ICustomerQueries
{
    Task<(int TotalCount, List<CustomerListDto> Result)> GetCustomers(CustomerListOptions list, int start, int take, CancellationToken cancellationToken);
}

public class CustomerQueries(VmsDbContext context, IUserProvider userProvider) : ICustomerQueries
{
    public async Task<(int TotalCount, List<CustomerListDto> Result)> GetCustomers(
        CustomerListOptions list, int start, int take, CancellationToken cancellationToken)
    {
        var customers = context.Customers.AsNoTracking()
            .AsQueryable();

        switch (list)
        {
            case CustomerListOptions.All:
                break;
            default:
                throw new NotSupportedException($"Unknown list option '{list}'.");
        }

        int totalCount = await customers.CountAsync(cancellationToken);

        var result = await customers
            .Skip(start)
            .Take(take)
            .Select(x => new CustomerListDto(x.CompanyCode, x.Code, x.Name))
            .ToListAsync(cancellationToken);

        return (totalCount, result);

    }
}
