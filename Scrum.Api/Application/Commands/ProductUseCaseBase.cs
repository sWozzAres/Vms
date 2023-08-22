namespace Scrum.Api.Application.Commands;

public class ProductUseCaseBase(ScrumDbContext dbContext)
{
    protected readonly ScrumDbContext DbContext = dbContext;
    protected CancellationToken Ct = CancellationToken.None;

    private Product? _product;
    protected Product Product => _product ?? throw new InvalidOperationException(NotLoadedMessage);

    const string NotLoadedMessage = "Product is not loaded.";

    protected async Task<bool> LoadAsync(Guid productId)
    {
        _product = await DbContext.Products.FindAsync(new object[] { productId }, Ct);
            //?? throw new InvalidOperationException($"Failed to load product {productId}.");
        return _product is not null;
    }
}