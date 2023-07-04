using System.Diagnostics.Contracts;
using System.Net;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Polly;
using Vms.Application.UseCase;
using Vms.Domain.Entity;
using Vms.Domain.Infrastructure;
using Vms.Web.Shared;

namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class ServiceBookingController(ILogger<ServiceBookingController> logger, VmsDbContext context) : ControllerBase
{
    readonly ILogger<ServiceBookingController> _logger = logger;
    readonly VmsDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    [HttpGet]
    [Route("{id}/suppliers")]
    public async Task<IActionResult> GetSuppliers(Guid id, CancellationToken cancellationToken)
    {
        var serviceBooking = await _context.ServiceBookings.FindAsync(id, cancellationToken);
        if (serviceBooking is null)
        {
            return NotFound();
        }

        var vehicle = await _context.Vehicles.FindAsync(serviceBooking.VehicleId, cancellationToken);
        if (vehicle is null)
        {
            return NotFound();
        }

        var suppliers = await _context.Suppliers.AsNoTracking()
            //.Where(s => s.Address.Location.Distance(vehicle.Address.Location) > 0)
            .OrderBy(s => s.Address.Location.Distance(vehicle.Address.Location))
            .Select(s => new SupplierLocatorDto(s.Code, s.Name, s.Address.Location.Distance(vehicle.Address.Location)))
            .ToListAsync(cancellationToken);

        return Ok(suppliers);
    }

    [HttpPost]
    public async Task<IActionResult> CreateServiceBooking(
        [FromBody] CreateServiceBookingDto request,
        CancellationToken cancellationToken)
    {
        var serviceBooking = await new CreateServiceBooking(_context)
            .CreateAsync(new CreateBookingRequest(request.VehicleId, null, null, null, false), cancellationToken);
        return CreatedAtAction("GetServiceBooking", new { id = serviceBooking.Id }, serviceBooking.ToDto());
    }

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetServiceBooking(Guid id,
        CancellationToken cancellationToken)
    {
        var serviceBooking = await _context.ServiceBookings.AsNoTracking()
                .SingleOrDefaultAsync(v => v.Id == id, cancellationToken);

        return serviceBooking is null ? NotFound() : Ok(serviceBooking.ToDto());
    }

    [HttpGet]
    [Route("{id}")]
    [AcceptHeader("application/vnd.full")]
    [ProducesResponseType(typeof(VehicleFullDto), StatusCodes.Status200OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetServiceBookingFull(Guid id,
        CancellationToken cancellationToken)
    {
        var serviceBooking = await _context.ServiceBookings.AsNoTracking()
            .Include(s => s.Vehicle).ThenInclude(v => v.VehicleVrm)
            .Include(s => s.Supplier).ThenInclude(s => s.Supplier)
            .SingleOrDefaultAsync(v => v.Id == id, cancellationToken);

        return serviceBooking is null ? NotFound() : Ok(serviceBooking.ToFullDto());
    }

    [HttpPut]
    [Route("{id}")]
    [ActionName("Put")]
    public async Task<IActionResult> Put([FromRoute] Guid id,
        [FromBody] ServiceBookingDto request,
        CancellationToken cancellationToken)
    {
        if (request.Id != id)
        {
            return BadRequest();
        }

        var serviceBooking = await _context.ServiceBookings.FindAsync(id, cancellationToken);
        if (serviceBooking is null)
        {
            return NotFound();
        }

        serviceBooking.PreferredDate1 = request.PreferredDate1;
        serviceBooking.PreferredDate2 = request.PreferredDate2;
        serviceBooking.PreferredDate3 = request.PreferredDate3;

        return Ok(serviceBooking.ToDto());
    }
}

public static partial class DomainExtensions
{
    public static ServiceBookingDto ToDto(this ServiceBooking serviceBooking)
    {
        return new ServiceBookingDto()
        {
            Id = serviceBooking.Id,
            VehicleId = serviceBooking.VehicleId,
            CompanyCode = serviceBooking.CompanyCode,
            PreferredDate1 = serviceBooking.PreferredDate1,
            PreferredDate2 = serviceBooking.PreferredDate2,
            PreferredDate3 = serviceBooking.PreferredDate3,
        };
    }

    public static ServiceBookingFullDto ToFullDto(this ServiceBooking serviceBooking)
    {
        return new ServiceBookingFullDto(
            serviceBooking.Id,
            serviceBooking.VehicleId,
            serviceBooking.CompanyCode,
            serviceBooking.Vehicle.VehicleVrm.Vrm,
            serviceBooking.Vehicle.Make,
            serviceBooking.Vehicle.Model,
            serviceBooking.PreferredDate1,
            serviceBooking.PreferredDate2,
            serviceBooking.PreferredDate3,
            serviceBooking.Supplier?.Supplier.ToSupplierShortDto()
        );
    }

    public static SupplierShortDto ToSupplierShortDto(this Supplier supplier)
        => new(supplier.Code, supplier.Name);
}