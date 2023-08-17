namespace Catalog.Api.Controllers;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class ProductController(ILogger<ProductController> logger, CatalogDbContext context) : ControllerBase
{
    #region CRUD
    [HttpGet]
    public async Task<IActionResult> Get(
        ProductListOptions list, int start, int take,
        [FromServices] IProductQueries queries,
        CancellationToken cancellationToken)
    {
        var (totalCount, result) = await queries.GetProducts(list, start, take, cancellationToken);
        return Ok(new ListResult<ProductListDto>(totalCount, result));
    }
    [HttpGet]
    [Route("{id:guid}")]
    [AcceptHeader("application/vnd.full")]
    public async Task<IActionResult> GetProductFull(Guid id,
        [FromServices] IProductQueries queries,
        [FromServices] IRecentViewLogger<CatalogDbContext> recentViewLogger,
        CancellationToken cancellationToken)
    {
        var product = await queries.GetProductFull(id, cancellationToken);
        if (product is null)
        {
            return NotFound();
        }

        await recentViewLogger.LogAsync(product.Id);

        await context.SaveChangesAsync(cancellationToken);

        return Ok(product);
    }
    #endregion
}