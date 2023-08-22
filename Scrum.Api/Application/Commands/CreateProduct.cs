namespace Scrum.Api.Application.Commands;

public class CreateProduct(ScrumDbContext dbContext) : ProductUseCaseBase(dbContext)
{
    public Product Create(CreateProductRequest command)
    {
        var product = new Product(command.Name);
        DbContext.Products.Add(product);

        return product;
    }
}
