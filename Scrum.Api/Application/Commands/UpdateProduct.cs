namespace Scrum.Api.Application.Commands;

public class UpdateProduct(
    ScrumDbContext dbContext,
    ILogger<UpdateProduct> logger) : ProductUseCaseBase(dbContext) 
{
    public async Task<bool> UpdateAsync(Guid productId, UpdateProductRequest request, CancellationToken ct)
    {
        Ct = ct;

        logger.LogInformation("Updating product {productid}, request {@updateproductrequest}.", productId, request);

        if (!await LoadAsync(productId))
            throw new InvalidOperationException($"Product {productId} not found.");

        Product.Name = request.Name;
        
        logger.LogInformation("State {state}.", DbContext.Entry(Product).State);

        return DbContext.Entry(Product).State == EntityState.Modified;
    }
}
