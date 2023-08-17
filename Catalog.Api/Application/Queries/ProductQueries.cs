namespace Catalog.Api.Application.Queries;

public interface IProductQueries
{
    Task<ActivityLogDto?> GetActivity(Guid id, Guid activityId, CancellationToken cancellationToken);
    Task<ProductFullDto?> GetProductFull(Guid id, CancellationToken cancellationToken);
    Task<(int TotalCount, List<ProductListDto> Result)> GetProducts(ProductListOptions list, int start, int take, CancellationToken cancellationToken);
}

public class ProductQueries(CatalogDbContext context, IUserProvider userProvider) : IProductQueries
{
    public async Task<ActivityLogDto?> GetActivity(Guid id, Guid activityId,
        CancellationToken cancellationToken)
    {
        var activityLog = await (from sb in context.Products
                                 join ac in context.ActivityLog on sb.Id equals ac.DocumentId
                                 where sb.Id == id && ac.DocumentId == activityId
                                 select ac).SingleOrDefaultAsync(cancellationToken);

        return activityLog?.ToDto();
    }
    public async Task<ProductFullDto?> GetProductFull(Guid id,
        CancellationToken cancellationToken)
    {
        var query = from s in context.Products.AsNoTracking()
                    where s.Id == id
                    join _f in context.Followers on new { s.Id, userProvider.UserId } equals new { Id = _f.DocumentId, _f.UserId } into __f
                    from f in __f.DefaultIfEmpty()
                    select new
                    {
                        s.Id,
                        s.Code,
                        s.Description,
                        IsFollowing = f.UserId != default,
                    };

        var product = await query.SingleOrDefaultAsync(cancellationToken);
        if (product is null)
            return null;

        var dto = new ProductFullDto(product.Id, product.Code, product.Description,
            product.IsFollowing);

        return dto;
    }
    public async Task<(int TotalCount, List<ProductListDto> Result)> GetProducts(
        ProductListOptions list, int start, int take, CancellationToken cancellationToken)
    {
        var products = context.Products.AsNoTracking()
            .AsQueryable();
        products = list switch
        {
            ProductListOptions.All => from v in products
                                      orderby v.Id
                                      select v,
            ProductListOptions.Following => from x in products
                                            join f in context.Followers on x.Id equals f.DocumentId
                                            where f.UserId == userProvider.UserId
                                            orderby f.Id
                                            select x,
            ProductListOptions.Recent => from x in products
                                         join f in context.RecentViews on x.Id equals f.DocumentId
                                         where f.UserId == userProvider.UserId
                                         orderby f.ViewDate descending
                                         select x,
            _ => throw new NotSupportedException($"Unknown list option '{list}'."),
        };
        int totalCount = await products.CountAsync(cancellationToken);

        var result = await products
            .Skip(start)
            .Take(take)
            .Select(x => new ProductListDto(x.Id, x.Code, x.Description))
            .ToListAsync(cancellationToken);

        return (totalCount, result);
    }
}
