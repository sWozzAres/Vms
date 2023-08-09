namespace Vms.Application.Queries;

public interface ISupplierQueries
{
    Task<(int TotalCount, List<SupplierListDto> Result)> GetSuppliers(SupplierListOptions list, int start, int take, CancellationToken cancellationToken);
}

public class SupplierQueries(VmsDbContext context, IUserProvider userProvider) : ISupplierQueries
{
    public async Task<(int TotalCount, List<SupplierListDto> Result)> GetSuppliers(
        SupplierListOptions list, int start, int take, CancellationToken cancellationToken)
    {
        var suppliers = context.Suppliers.AsNoTracking()
            .AsQueryable();

        switch (list)
        {
            case SupplierListOptions.All:
                break;
            default:
                throw new NotSupportedException($"Unknown list option '{list}'.");
        }

        int totalCount = await suppliers.CountAsync(cancellationToken);

        var result = await suppliers
            .Skip(start)
            .Take(take)
            .Select(x => new SupplierListDto(x.Code, x.Name))
            .ToListAsync(cancellationToken);

        return (totalCount, result);

    }
}
