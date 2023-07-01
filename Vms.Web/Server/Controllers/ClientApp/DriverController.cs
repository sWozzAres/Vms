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
public class DriverController(VmsDbContext context) : ControllerBase
{
    readonly VmsDbContext _context = context;

    [HttpGet]
    [Route("{filter}")]
    [AcceptHeader("application/vnd.drivershort")]
    [ProducesResponseType(typeof(DriverShortDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDriversShort(string filter, CancellationToken cancellationToken)
        => Ok(await _context.Drivers.AsNoTracking()
                .Where(d => d.LastName.StartsWith(filter))
                .Select(d => new DriverShortDto(d.Id, d.CompanyCode, d.EmailAddress, d.FullName, d.MobileNumber))
                .ToListAsync(cancellationToken));

    [HttpGet]
    [Route("{id:guid}")]
    [AcceptHeader("application/vnd.drivershort")]
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

        return Ok(new DriverShortDto(driver.Id, driver.CompanyCode, driver.EmailAddress, driver.FullName, driver.MobileNumber));
    }
}
