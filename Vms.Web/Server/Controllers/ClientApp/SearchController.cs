using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vms.Domain.Infrastructure;
using Vms.Web.Shared;

namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class SearchController(VmsDbContext context) : ControllerBase
{
    [HttpGet]
    [Route("{searchString}")]
    public async Task<IActionResult> Search(string searchString, CancellationToken cancellationToken)
    {
        var results = await (from tag in context.EntityTags
                      where EF.Functions.FreeText(tag.Content, searchString)
                      select new EntityTagDto(tag.EntityKey, (EntityTagKindDto)tag.EntityKind, tag.Name)).ToListAsync(cancellationToken);

        return Ok(results);
    }
}
