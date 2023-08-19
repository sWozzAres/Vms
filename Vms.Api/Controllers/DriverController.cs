using Vms.Application.Extensions;

namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class DriverController(VmsDbContext context) : ControllerBase
{
    readonly VmsDbContext _context = context;

    [HttpGet]
    [Route("{filter}")]
    [AcceptHeader("application/vnd.short")]
    [ProducesResponseType(typeof(DriverShortDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDriversShort(string filter, CancellationToken cancellationToken)
        => Ok(await _context.Drivers.AsNoTracking()
                .Where(d => d.LastName.StartsWith(filter))
                .Select(d => d.ToShortDto())
                .ToListAsync(cancellationToken));

    [HttpGet]
    [Route("{id:guid}")]
    [AcceptHeader("application/vnd.short")]
    [ProducesResponseType(typeof(DriverShortDto), StatusCodes.Status200OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetDriverShort(Guid id, CancellationToken cancellationToken)
    {
        var driver = await _context.Drivers.AsNoTracking()
            .SingleOrDefaultAsync(d => d.Id == id, cancellationToken);
        if (driver is null)
        {
            return NotFound();
        }

        return Ok(driver.ToShortDto());
    }

    [HttpGet]
    public async Task<IActionResult> Get(
        DriverListOptions list, int start, int take,
        [FromServices] DriverQueries queries,
        CancellationToken cancellationToken)
    {
        var (totalCount, result) = await queries.GetDrivers(list, start, take, cancellationToken);
        return Ok(new ListResult<DriverListDto>(totalCount, result));
    }
}
