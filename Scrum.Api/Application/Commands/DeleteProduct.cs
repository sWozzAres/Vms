namespace Scrum.Api.Application.Commands;

public class DeleteProduct(ScrumDbContext dbContext) : ProductUseCaseBase(dbContext)
{
    public async Task<bool> DeleteAsync(Guid productId, CancellationToken ct)
    {
        Ct = ct;

        var loaded = await LoadAsync(productId);
        if (loaded)
            DbContext.Products.Remove(Product);

        return loaded;
    }
}
