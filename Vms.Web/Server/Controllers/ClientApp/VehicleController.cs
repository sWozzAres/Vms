﻿using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using Utopia.Blazor.Application.Shared;
using Vms.Application;
using Vms.Application.Commands.VehicleUseCase;
using Vms.Application.Queries;
using Vms.Application.Services;
using Vms.Domain.Common;
using Vms.Domain.Infrastructure;
using Vms.Web.Shared;

namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class VehicleController(ILogger<VehicleController> logger, VmsDbContext context) : ControllerBase
{
    #region Follow
    [HttpPost]
    [Route("{id}/follow")]
    public async Task<IActionResult> Follow(Guid id,
        [FromServices] IFollowVehicle follow,
        CancellationToken cancellationToken)
    {
        await follow.FollowAsync(id, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpDelete]
    [Route("{id}/follow")]
    public async Task<IActionResult> Unfollow(Guid id,
        [FromServices] IUnfollowVehicle unfollow,
        CancellationToken cancellationToken)
    {
        if (!await unfollow.UnfollowAsync(id, cancellationToken))
            return NotFound();

        await context.SaveChangesAsync(cancellationToken);
        return Ok();
    }
    #endregion
    [HttpGet]
    [Route("{id}/events/{serviceBookingId?}")]
    public async Task<IActionResult> GetEvents(Guid id,
        Guid? serviceBookingId,
        CancellationToken cancellationToken)
    {
        var motEvents = await context.MotEvents
            .Where(e => e.VehicleId == id && e.ServiceBookingId == serviceBookingId)
            .Select(e => new MotEventDto(e.Id, e.Due))
            .ToListAsync(cancellationToken);

        return Ok(new VehicleEvents(motEvents));
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
        var vehicle = await context.Vehicles.AsNoTracking()
                .SingleOrDefaultAsync(v => v.Id == id, cancellationToken);

        return vehicle is null ? NotFound() : Ok(vehicle.ToDto());

    }
    [HttpGet]
    [Route("{id}")]
    [AcceptHeader("application/vnd.vehiclefull")]
    [ProducesResponseType(typeof(VehicleFullDto), StatusCodes.Status200OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetVehicleFull(Guid id,
        [FromServices] IVehicleQueries queries,
        [FromServices] IRecentViewLogger recentViewLogger,
        CancellationToken cancellationToken)
    {
        var vehicle = await queries.GetVehicleFull(id, cancellationToken);
        if (vehicle is null)
        {
            return NotFound();
        }

        await recentViewLogger.LogAsync(vehicle.Id);
        await context.SaveChangesAsync(cancellationToken);

        return Ok(vehicle);
    }

    [HttpGet]
    [ProducesResponseType(typeof(ListResult<VehicleListDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVehicles(VehicleListOptions list, int start, int take,
        [FromServices] IVehicleQueries queries,
        CancellationToken cancellationToken)
    {
        var (totalCount, result) = await queries.GetVehicles(list, start, take, cancellationToken);
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
        var r = await new RemoveDriverFromVehicle(context).RemoveAsync(id, driverId, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);

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
        await new AddDriverToVehicle(context)
            .AddAsync(id, request, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
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
        await new AssignCustomerToVehicle(context)
            .AssignAsync(id, request, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpDelete]
    [Route("{id}/customer")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveCustomerFromVehicle(
        Guid id,
        CancellationToken cancellationToken)
    {
        await new RemoveCustomerFromVehicle(context)
            .RemoveAsync(id, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
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
        await new AssignFleetToVehicle(context)
            .AssignAsync(id, request, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpDelete]
    [Route("{id}/fleet")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> RemoveFleetFromVehicle(
        Guid id,
        CancellationToken cancellationToken)
    {
        await new RemoveFleetFromVehicle(context)
            .RemoveAsync(id, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
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

        await context.SaveChangesAsync(cancellationToken);

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

        logger.LogDebug("Put vehicle, id {id}.", id);

        var vehicle = await context.Vehicles
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
                context.MotEvents.Remove(mot);
        }
        else
        {
            if (request.MotDue.HasValue)
                //vehicle.MotEvents.Add(new(vehicle.CompanyCode, vehicle.Id, request.MotDue.Value, true));
                context.MotEvents.Add(new(vehicle.CompanyCode, vehicle.Id, request.MotDue.Value, true));
        }

        vehicle.Address = new Address(request.Address.Street, request.Address.Locality, request.Address.Town, request.Address.Postcode,
            new Point(request.Address.Location.Longitude, request.Address.Location.Latitude) { SRID = 4326 });

        await context.SaveChangesAsync(cancellationToken);

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
