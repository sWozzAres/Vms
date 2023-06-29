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
    public async Task<IActionResult> GetVehicle(Guid id,
        [FromServices] VmsDbContext context,
        CancellationToken cancellationToken)
    {
        var vehicle = await context.Vehicles.AsNoTracking()
                .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        return vehicle is null ? NotFound() : Ok(vehicle.ToDto());

    }
    [HttpGet]
    [Route("{id}")]
    [AcceptHeader("application/vnd.vehiclefull")]
    [ProducesResponseType(typeof(VehicleFullDto), StatusCodes.Status200OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetVehicleFull(Guid id,
        [FromServices] VmsDbContext context,
        CancellationToken cancellationToken)
    {
        var vehicle = await context.Vehicles.AsNoTracking()
                .Include(v => v.C)
                .Include(v => v.Fleet)
                .Include(v => v.DriverVehicles).ThenInclude(dv => dv.EmailAddressNavigation)
                .FirstOrDefaultAsync(v => v.Id == id, cancellationToken);

        return vehicle is null ? NotFound() : Ok(vehicle.ToFullDto());
    }
    [HttpDelete]
    [Route("{id}/drivers/{emailAddress}")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    public async Task<IActionResult> RemoveDriverFromVehicle(
        Guid id, string emailAddress,
        [FromServices] VmsDbContext context,
        CancellationToken cancellationToken)
    {
        var entry = await context.DriverVehicles.FindAsync(new object[] { emailAddress, id }, cancellationToken);
        if (entry is null)
        {
            return NotFound();
        }

        context.DriverVehicles.Remove(entry);
        await context.SaveChangesAsync();

        return Accepted();
    }

    [HttpPut]
    [Route("{id}")]
    [ActionName("PutAsync")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> PutAsync([FromRoute] Guid id,
        [FromServices] VmsDbContext context,
        [FromBody] VehicleDto request,
        CancellationToken cancellationToken)
    {
        if(request.Id != id)
        {
            return BadRequest();
        }
        
        _logger.LogInformation("Put vehicle, id {id}.", id);

        var vehicle = await context.Vehicles.FindAsync(id, cancellationToken);
        if (vehicle is null)
        {
            return NotFound();
        }

        vehicle.Vrm = request.Vrm;
        vehicle.UpdateModel(request.Make, request.Model);

        
        if (context.Entry(vehicle).State == EntityState.Modified || context.Entry(vehicle.VehicleVrm).State == EntityState.Modified)
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
    public static DriverFullDto ToFullDto(this Driver driver)
        => new(driver.EmailAddress, driver.FullName(), driver.MobileNumber);

    public static VehicleFullDto ToFullDto(this Vehicle vehicle)
    => new(
            vehicle.CompanyCode,
            vehicle.Id,
            vehicle.Vrm,
            vehicle.Make,
            vehicle.Model,
            vehicle.ChassisNumber,
            vehicle.DateFirstRegistered,
            vehicle.Address.ToFullDto(),
            vehicle.C is null ? null : new CustomerSummaryResource(vehicle.C.Code, vehicle.C.Name),
            vehicle.Fleet is null ? null : new FleetSummaryResource(vehicle.Fleet.Code, vehicle.Fleet.Name),
            vehicle.DriverVehicles.Select(x=>x.EmailAddressNavigation.ToFullDto()).ToList()
            );
    

    public static AddressFullDto ToFullDto(this Address address)
        => new(address.Street, address.Locality, address.Town, address.Postcode, address.Location.ToFullDto());

    public static GeometryFullDto ToFullDto(this Geometry geometry)
        => new(geometry.Coordinate.X, geometry.Coordinate.Y);

    public static VehicleDto ToDto(this Vehicle vehicle)
        => new(
            vehicle.CompanyCode,
            vehicle.Id,
            vehicle.Vrm,
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