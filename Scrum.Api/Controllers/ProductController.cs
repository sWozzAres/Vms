using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Scrum.Api.Application.Commands;
using Scrum.Api.Application.Queries;
using Scrum.Api.Domain.Configuration;
using Scrum.Api.Exceptions;

namespace Scrum.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ProductController(ScrumDbContext dbContext) : ControllerBase
{
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> CreateProduct(
        CreateProductRequest request,
        [FromServices] CreateProduct command,
        CancellationToken ct)
    {
        var product = command.Create(request);

        try
        {
            await dbContext.SaveChangesAsync(ct);
        } 
        catch(DbUpdateException dbe) 
            when (dbe.InnerException is SqlException se && se.Message.Contains(ProductEntityTypeConfiguration.IX_Product_Name))
        {
            throw new ScrumDomainException($"A product with the name '{request.Name}' already exists.");
        }

        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, new { id = product.Id });
    }

    [HttpGet]
    [Route("{id:guid}")]
    public async Task<IActionResult> GetProduct(
        Guid id,
        [FromServices] ProductQueries queries,
        CancellationToken ct)
    => Ok(await queries.GetProductAsync(id, ct));

    [HttpPost]
    [Route("{id:guid}")]
    public async Task<IActionResult> UpdateProduct(
        Guid id,
        [FromBody] UpdateProductRequest request,
        [FromServices] UpdateProduct command,
        CancellationToken ct)
    {
        if (await command.UpdateAsync(id, request, ct))
        {
            try
            {
                await dbContext.SaveChangesAsync(ct);
            }
            catch (DbUpdateException dbe)
                when (dbe.InnerException is SqlException se && se.Message.Contains(ProductEntityTypeConfiguration.IX_Product_Name))
            {
                throw new ScrumDomainException($"A product with the name '{request.Name}' already exists.");
            }
        }

        return Ok();
    }

    [HttpDelete]
    [Route("{id:guid}")]
    public async Task<IActionResult> DeleteProduct(
        Guid id,
        [FromServices] DeleteProduct command,
        CancellationToken ct)
    {
        var deleted = await command.DeleteAsync(id, ct);
        if (deleted)
            await dbContext.SaveChangesAsync(ct);

        return deleted ? Ok() : NotFound();
    }

    [HttpGet]
    public async Task<IActionResult> GetProducts(
        [FromServices] ProductQueries queries,
        CancellationToken ct)
    => Ok(await queries.GetProductsAsync(ct));
}
