using Vms.Domain.Core;

namespace Vms.Application.Queries;

public interface ISupplierQueries
{
    Task<SupplierFullDto?> GetSupplierFull(string code, CancellationToken cancellationToken);
    Task<(int TotalCount, List<SupplierListDto> Result)> GetSuppliers(SupplierListOptions list, int start, int take, CancellationToken cancellationToken);
}

public class SupplierQueries(VmsDbContext context, IUserProvider userProvider) : ISupplierQueries
{
    public async Task<SupplierFullDto?> GetSupplierFull(string code,
        CancellationToken cancellationToken)
    {
        var query = from s in context.Suppliers.AsNoTracking()
                    where s.Code == code
                    select new
                    {
                        s.Code,
                        s.Name,
                        s.IsIndependent,
                        s.Address.Street,
                        s.Address.Locality,
                        s.Address.Town,
                        s.Address.Postcode,
                        s.Address.Location.Coordinate.Y,
                        s.Address.Location.Coordinate.X,
                    };

        var supplier = await query.SingleOrDefaultAsync(cancellationToken);
        if (supplier is null)
            return null;

        var dto = new SupplierFullDto(supplier.Code, supplier.Name, supplier.IsIndependent,
        new(supplier.Street, supplier.Locality, supplier.Town, supplier.Postcode, new(supplier.Y, supplier.X)));

        return dto;
    }
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
            .OrderBy(s => s.Code)
            .Skip(start)
            .Take(take)
            .Select(x => new SupplierListDto(x.Code, x.Name))
            .ToListAsync(cancellationToken);

        return (totalCount, result);

    }
}
