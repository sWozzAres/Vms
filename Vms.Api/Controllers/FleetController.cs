using Vms.Application.Extensions;

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