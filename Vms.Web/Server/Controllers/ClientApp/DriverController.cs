using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Polly;
using Vms.Domain.Infrastructure;
using Vms.Web.Shared;

namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class DriverController : ControllerBase
{
    [HttpGet]
    [Route("{filter}")]
    [AcceptHeader("application/vnd.drivershort")]
    [ProducesResponseType(typeof(DriverShortDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDriversShort(string filter,
        [FromServices] VmsDbContext context,
        CancellationToken cancellationToken)
    {
        var drivers = await context.Drivers.AsNoTracking()
                .Where(d => d.LastName.StartsWith(filter))
                .Select(d=>new DriverShortDto(d.EmailAddress, d.FullName()))
                .ToListAsync(cancellationToken);

        return Ok(drivers);
    }
    [HttpGet]
    [Route("{emailAddress}")]
    [AcceptHeader("application/vnd.driverfull")]
    [ProducesResponseType(typeof(DriverFullDto), StatusCodes.Status200OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetDriverShort(string emailAddress,
        [FromServices] VmsDbContext context,
        CancellationToken cancellationToken)
    {
        var driver = await context.Drivers.AsNoTracking()
            .FirstOrDefaultAsync(d => d.EmailAddress == emailAddress, cancellationToken);
            
        if (driver is null)
        {
            return NotFound();
        }

        return Ok(new DriverFullDto(driver.EmailAddress, driver.FullName(), driver.MobileNumber));
    }
}
