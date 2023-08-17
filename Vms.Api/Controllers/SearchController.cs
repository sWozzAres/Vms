namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class SearchController(VmsDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search(string? term, CancellationToken cancellationToken)
    {
        if (term is null)
        {
            return NotFound();
        }

        var results = await (from tag in context.EntityTags
                             where EF.Functions.FreeText(tag.Content, term)
                             select new EntityTagDto(tag.EntityKey, (EntityTagKindDto)tag.EntityKind, tag.Name)).ToListAsync(cancellationToken);

        return Ok(results);
    }
}
