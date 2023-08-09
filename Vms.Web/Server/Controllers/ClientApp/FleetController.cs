using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utopia.Blazor.Application.Shared;
using Vms.Application.Queries;
using Vms.Domain.Core;
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
                .Select(d => d.ToShortDto())
                .ToListAsync(cancellationToken));

    [HttpGet]
    public async Task<IActionResult> Get(
        FleetListOptions list, int start, int take,
        [FromServices] IFleetQueries queries,
        CancellationToken cancellationToken)
    {
        var (totalCount, result) = await queries.GetFleets(list, start, take, cancellationToken);
        return Ok(new ListResult<FleetListDto>(totalCount, result));
    }
}

public static partial class DomainExtensions
{
    public static FleetShortDto ToShortDto(this Fleet fleet) => new(fleet.CompanyCode, fleet.Code, fleet.Name);
}