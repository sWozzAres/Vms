using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Polly;
using Vms.Domain.Entity;
using Vms.Domain.Infrastructure;
using Vms.Web.Shared;

namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class VehicleController : ControllerBase
{
    private readonly ILogger<CompanyController> _logger;

    public VehicleController(ILogger<CompanyController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<VehicleDto>> GetVehicle(Guid id,
        [FromServices] VmsDbContext context,
        CancellationToken cancellationToken)
    {
        var vehicle = await context.Vehicles.FindAsync(new[] { id }, cancellationToken);
        if (vehicle is null)
        {
            return NotFound();
        }

        return vehicle.ToDto();
    }

    [HttpPatch]
    [Route("{id}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> PatchAsync([FromRoute] Guid id,
        [FromServices] VmsDbContext context,
        [FromBody] VehicleModel request,
        CancellationToken cancellationToken)
    {
        var vehicle = await context.Vehicles.FindAsync(new[] { id }, cancellationToken);
        if (vehicle is null)
        {
            return NotFound();
        }

        //vehicle.Name = request.Name;

        if (context.Entry(vehicle).State == EntityState.Modified)
        {
            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogInformation("Concurrency violation while modifying vehicle, id '{id}'.", id);

                //if (rowVersion is not null)
                //    return StatusCode(StatusCodes.Status412PreconditionFailed);

                //TODO retry?
                return Problem();
            }

            //HttpContext.SetETag(product.RowVersion);

            return Ok(vehicle.ToDto());
        }

        // return NoContent to signify no changes
        return NoContent();
    }
}
public static partial class DomainExtensions
{
    public static VehicleDto ToDto(this Vehicle vehicle)
        => new(
            vehicle.CompanyCode,
            vehicle.Id,
            vehicle.Make,
            vehicle.Model,
            vehicle.ChassisNumber,
            vehicle.DateFirstRegistered,
            vehicle.Address.ToDto(),
            vehicle.CustomerCode,
            vehicle.FleetCode
            );

    public static AddressDto ToDto(this Address address)
        => new(address.Street, address.Locality, address.Town, address.Postcode, address.Location.ToDto());

    public static GeometryDto ToDto(this Geometry geometry)
        => new(geometry.Coordinate.X, geometry.Coordinate.Y);
}