using Microsoft.EntityFrameworkCore;

namespace Scrum.Api.Application.Queries;

public class ProductQueries(ScrumDbContext dbContext)
{
    public async Task<ProductFullDto> GetProductAsync(Guid productId, CancellationToken ct)
    {
        var query = from p in dbContext.Products
                    where p.Id == productId
                    select new
                    {
                        p.Id,
                        p.Name,
                    };

        var product = await query.AsNoTracking()
            .SingleAsync(ct);

        return new(product.Id, product.Name);
    }
    public async Task<List<ProductListDto>> GetProductsAsync(CancellationToken ct)
    {
        var query = from p in dbContext.Products
                    select new
                    {
                        p.Id,
                        p.Name,
                    };

        var products = await query.AsNoTracking()
            .ToListAsync(ct);

        return products.Select(product => new ProductListDto(product.Id, product.Name)).ToList();
    }
}
