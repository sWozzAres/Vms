﻿using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Utopia.Blazor.Application.Shared;
using Vms.Application.Services;
using Vms.Application.UseCase;
using Vms.Application.UseCase.VehicleUseCase;
using Vms.Domain.Entity;
using Vms.Domain.Exceptions;
using Vms.Domain.Infrastructure;
using Vms.Web.Shared;

namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class VehicleController(ILogger<VehicleController> logger, VmsDbContext context) : ControllerBase
{
    readonly ILogger<VehicleController> _logger = logger;
    readonly VmsDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    [HttpGet]
    [Route("{id}/openevents")]
    public async Task<IActionResult> GetOpenEvents(Guid id, CancellationToken cancellationToken)
    {
        var motEvents = await _context.MotEvents
            .Where(e => e.VehicleId == id && e.ServiceBookingId == null)
            .Select(e => new OpenMotEvent(e.Id, e.Due))
            .ToListAsync(cancellationToken);

        return Ok(new OpenEvents(motEvents));
    }

    //[HttpGet]
    //[Route("{id}/servicebookings")]
    //[AcceptHeader("application/vnd.full")]
    //public async Task<IActionResult> GetServiceBookingsFull(Guid id,
    //    CancellationToken cancellationToken)
    //{
    //    var serviceBookings = await _context.ServiceBookings.AsNoTracking()
    //            .Include(s => s.Vehicle).ThenInclude(v => v.VehicleVrm)
    //            .Include(s => s.Supplier)
    //            .Where(v => v.VehicleId == id)
    //            .Select(v => v.ToFullDto())
    //            .ToListAsync(cancellationToken);

    //    return Ok(serviceBookings);
    //}

    [HttpGet]
    [Route("{id}")]
    [ProducesResponseType(typeof(VehicleDto), StatusCodes.Status200OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetVehicle(Guid id,
        CancellationToken cancellationToken)
    {
        var vehicle = await _context.Vehicles.AsNoTracking()
                .SingleOrDefaultAsync(v => v.Id == id, cancellationToken);

        return vehicle is null ? NotFound() : Ok(vehicle.ToDto());

    }
    [HttpGet]
    [Route("{id}")]
    [AcceptHeader("application/vnd.vehiclefull")]
    [ProducesResponseType(typeof(VehicleFullDto), StatusCodes.Status200OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetVehicleFull(Guid id,
        CancellationToken cancellationToken)
    {
        var vehicle = await _context.Vehicles.AsNoTracking()
                .Include(v => v.C)
                .Include(v => v.Fleet)
                .Include(v => v.DriverVehicles).ThenInclude(dv => dv.Driver)
                .Include(v=>v.MotEvents.Where(e=>e.IsCurrent))
                .SingleOrDefaultAsync(v => v.Id == id, cancellationToken);

        return vehicle is null ? NotFound() : Ok(vehicle.ToFullDto());
    }

    [HttpGet]
    //[Route("{list}/{start}/{take}")]
    [ProducesResponseType(typeof(ListResult<VehicleListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVehicles(int list, int start, int take, CancellationToken cancellationToken)
    {
        int totalCount = await context.Vehicles.CountAsync(cancellationToken);

        var result = await context.Vehicles
            .Skip(start)
            .Take(take)
            .Select(x => new VehicleListDto(x.Id, x.CompanyCode, x.Vrm, x.Make, x.Model))
            .ToListAsync(cancellationToken);

        return Ok(new ListResult<VehicleListDto>(totalCount, result));
    }

    [HttpDelete]
    [Route("{id}/drivers/{driverId}")]
    [ProducesResponseType((int)HttpStatusCode.Accepted)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> RemoveDriverFromVehicle(
        Guid id, Guid driverId,
        CancellationToken cancellationToken)
    {
        var r = await new RemoveDriverFromVehicle(_context).RemoveAsync(id, driverId, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);
            
        return r ? Accepted()
            : NotFound();
    }

    [HttpPost]
    [Route("{id}/drivers")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AddDriverToVehicle(
        Guid id,
        [FromBody] AddDriverToVehicleCommand request,
        CancellationToken cancellationToken)
    {
        await new AddDriverToVehicle(_context)
            .AddAsync(id, request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpPost]
    [Route("{id}/customer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignCustomerToVehicle(
        Guid id,
        [FromBody] AssignCustomerToVehicleCommand request,
        CancellationToken cancellationToken)
    {
        await new AssignCustomerToVehicle(_context)
            .AssignAsync(id, request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpDelete]
    [Route("{id}/customer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveCustomerFromVehicle(
        Guid id,
        CancellationToken cancellationToken)
    {
        await new RemoveCustomerFromVehicle(_context)
            .RemoveAsync(id, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpPost]
    [Route("{id}/fleet")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> AssignFleetToVehicle(
        Guid id,
        [FromBody] AssignFleetToVehicleCommand request,
        CancellationToken cancellationToken)
    {
        await new AssignFleetToVehicle(_context)
            .AssignAsync(id, request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpDelete]
    [Route("{id}/fleet")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveFleetFromVehicle(
        Guid id,
        CancellationToken cancellationToken)
    {
        await new RemoveFleetFromVehicle(_context)
            .RemoveAsync(id, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    

    [HttpPost]
    public async Task<IActionResult> CreateVehicle(
        [FromBody] VehicleDto vehicleDto,
        [FromServices] ICreateVehicle createVehicle,
        CancellationToken cancellationToken)
    {
        var request = new CreateVehicleRequest(vehicleDto.CompanyCode!, vehicleDto.Vrm, 
            vehicleDto.Make!, vehicleDto.Model!,
            vehicleDto.DateFirstRegistered, vehicleDto.MotDue,
            new Address(vehicleDto.Address.Street,
                  vehicleDto.Address.Locality,
                  vehicleDto.Address.Town,
                  vehicleDto.Address.Postcode,
                  new Point(vehicleDto.Address.Location.Longitude, vehicleDto.Address.Location.Latitude) { SRID = 4326 }),
            null, null);


        var vehicle = await createVehicle.CreateAsync(request, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction("GetVehicle", new { id = vehicle.Id }, vehicle.ToDto());
    }

    [HttpPut]
    [Route("{id}")]
    [ActionName("Put")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> Put([FromRoute] Guid id,
        [FromBody] VehicleDto request,
        CancellationToken cancellationToken)
    {
        if (request.Id != id)
        {
            return BadRequest();
        }

        _logger.LogInformation("Put vehicle, id {id}.", id);

        var vehicle = await _context.Vehicles
            .Include(v => v.MotEvents.Where(e => e.IsCurrent))
            .SingleOrDefaultAsync(v => v.Id == id, cancellationToken);
            //.FindAsync(id, cancellationToken);
        if (vehicle is null)
        {
            return NotFound();
        }

        vehicle.Vrm = request.Vrm;
        vehicle.UpdateModel(request.Make!, request.Model!);
        //vehicle.Mot.Due = request.MotDue;

        var mot = vehicle.MotEvents.FirstOrDefault();
        if (mot is not null)
        {
            if (request.MotDue.HasValue)
                mot.Due = request.MotDue.Value;
            else
                _context.MotEvents.Remove(mot);
        }
        else
        {
            if (request.MotDue.HasValue)
                //vehicle.MotEvents.Add(new(vehicle.CompanyCode, vehicle.Id, request.MotDue.Value, true));
                _context.MotEvents.Add(new(vehicle.CompanyCode, vehicle.Id, request.MotDue.Value, true));
        }

        vehicle.Address = new Address(request.Address.Street, request.Address.Locality, request.Address.Town, request.Address.Postcode,
            new Point(request.Address.Location.Longitude, request.Address.Location.Latitude) { SRID = 4326});

        await _context.SaveChangesAsync(cancellationToken);

        return Ok(vehicle.ToDto());

        //if (_context.Entry(vehicle).State == EntityState.Modified || _context.Entry(vehicle.VehicleVrm).State == EntityState.Modified)
        //{
        //    try
        //    {
        //        await _context.SaveChangesAsync(cancellationToken);
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        _logger.LogInformation("Concurrency violation while modifying vehicle, id '{id}'.", id);

        //        //if (rowVersion is not null)
        //        //    return StatusCode(StatusCodes.Status412PreconditionFailed);

        //        //TODO retry?
        //        return Problem();
        //    }

        //    //HttpContext.SetETag(product.RowVersion);

           
        //}

        //// return NoContent to signify no changes
        //return NoContent();
    }
}
public static partial class DomainExtensions
{
    
    public static VehicleFullDto ToFullDto(this Vehicle vehicle)
    => new(
            vehicle.CompanyCode,
            vehicle.Id,
            vehicle.Vrm,
            vehicle.Make,
            vehicle.Model,
            vehicle.ChassisNumber,
            vehicle.DateFirstRegistered,
            //vehicle.Mot.Due,
            vehicle.MotEvents.FirstOrDefault()?.Due,
            vehicle.Address.ToFullDto(),
            vehicle.C is null ? null : new CustomerShortDto(vehicle.CompanyCode, vehicle.C.Code, vehicle.C.Name),
            vehicle.Fleet is null ? null : new FleetShortDto(vehicle.CompanyCode, vehicle.Fleet.Code, vehicle.Fleet.Name),
            vehicle.DriverVehicles.Select(x => x.Driver.ToShortDto()).ToList()
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