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
public class FleetController(VmsDbContext context) : ControllerBase
{
    readonly VmsDbContext _context = context;

    [HttpGet]
    [Route("{filter}")]
    [AcceptHeader("application/vnd.short")]
    [ProducesResponseType(typeof(FleetShortDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFleetsShort(string filter, CancellationToken cancellationToken)
        => Ok(await _context.Fleets.AsNoTracking()
                .Where(d => d.Name.StartsWith(filter))
                .Select(d => new FleetShortDto(d.CompanyCode, d.Code, d.Name))
                .ToListAsync(cancellationToken));

    
}
