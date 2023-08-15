using Vms.Domain.Core;

namespace Vms.Application.Queries;

public interface ISupplierQueries
{
    Task<ActivityLogDto?> GetActivity(Guid id, Guid activityId, CancellationToken cancellationToken);
    Task<SupplierFullDto?> GetSupplierFull(string code, CancellationToken cancellationToken);
    Task<(int TotalCount, List<SupplierListDto> Result)> GetSuppliers(SupplierListOptions list, int start, int take, CancellationToken cancellationToken);
}

public class SupplierQueries(VmsDbContext context, IUserProvider userProvider) : ISupplierQueries
{
    public async Task<ActivityLogDto?> GetActivity(Guid id, Guid activityId,
        CancellationToken cancellationToken)
    {
        var activityLog = await (from sb in context.Suppliers
                                 join ac in context.ActivityLog on sb.Id equals ac.DocumentId
                                 where sb.Id == id && ac.DocumentId == activityId
                                 select ac).SingleOrDefaultAsync(cancellationToken);

        return activityLog?.ToDto();
    }
    public async Task<SupplierFullDto?> GetSupplierFull(string code,
        CancellationToken cancellationToken)
    {
        var query = from s in context.Suppliers.AsNoTracking()
                    where s.Code == code
                    join _f in context.Followers on new { s.Id, userProvider.UserId } equals new { Id = _f.DocumentId, _f.UserId } into __f
                    from f in __f.DefaultIfEmpty()
                    select new
                    {
                        s.Id,
                        s.Code,
                        s.Name,
                        s.IsIndependent,
                        s.Address.Street,
                        s.Address.Locality,
                        s.Address.Town,
                        s.Address.Postcode,
                        s.Address.Location.Coordinate.Y,
                        s.Address.Location.Coordinate.X,
                        IsFollowing = f.UserId != default,
                    };

        var supplier = await query.SingleOrDefaultAsync(cancellationToken);
        if (supplier is null)
            return null;

        var dto = new SupplierFullDto(supplier.Id, supplier.Code, supplier.Name, supplier.IsIndependent,
            new(supplier.Street, supplier.Locality, supplier.Town, supplier.Postcode, new(supplier.Y, supplier.X)),
            supplier.IsFollowing);

        return dto;
    }
    public async Task<(int TotalCount, List<SupplierListDto> Result)> GetSuppliers(
    SupplierListOptions list, int start, int take, CancellationToken cancellationToken)
    {
        var suppliers = context.Suppliers.AsNoTracking()
        .AsQueryable();
        suppliers = list switch
        {
            SupplierListOptions.All => from v in suppliers
                                       orderby v.Id
                                       select v,
            SupplierListOptions.Following => from x in suppliers
                                             join f in context.Followers on x.Id equals f.DocumentId
                                             where f.UserId == userProvider.UserId
                                             orderby f.Id
                                             select x,
            SupplierListOptions.Recent => from x in suppliers
                                          join f in context.RecentViews on x.Id equals f.DocumentId
                                          where f.UserId == userProvider.UserId
                                          orderby f.ViewDate descending
                                          select x,
            _ => throw new NotSupportedException($"Unknown list option '{list}'."),
        };
        int totalCount = await suppliers.CountAsync(cancellationToken);

        var result = await suppliers
            .Skip(start)
            .Take(take)
            .Select(x => new SupplierListDto(x.Code, x.Name))
            .ToListAsync(cancellationToken);

        return (totalCount, result);

    }
}
