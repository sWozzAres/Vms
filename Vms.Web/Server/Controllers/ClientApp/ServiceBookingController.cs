﻿using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vms.Application.Services;
using Vms.Application.UseCase;
using Vms.Domain.Entity;
using Vms.Domain.Entity.ServiceBookingEntity;
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

    [HttpPost]
    [Route("{id}/assignsupplier")]
    public async Task<IActionResult> AssignSupplier(Guid id,
        [FromBody] TaskAssignSupplierCommand request,
        CancellationToken cancellationToken)
    {
        await new AssignSupplierUseCase(_context)
                .Assign(id, request, cancellationToken);

        return Ok();
    }

    [HttpPost]
    [Route("{id}/unbooksupplier")]
    public async Task<IActionResult> UnbookSupplier(Guid id,
        [FromBody] TaskUnbookSupplierCommand request,
        CancellationToken cancellationToken)
    {
        await new UnbookSupplier(_context)
            .UnbookAsync(id, request, cancellationToken);

        return Ok();
    }

    [HttpPost]
    [Route("{id}/booksupplier")]
    public async Task<IActionResult> BookSupplier(Guid id,
        [FromBody] TaskBookSupplierCommand request,
        [FromServices] IBookSupplier bookSupplier,
        CancellationToken cancellationToken)
    {
        await bookSupplier.BookAsync(id, request, cancellationToken);
        return Ok();
    }

    [HttpPost]
    [Route("{id}/checkworkstatus")]
    public async Task<IActionResult> CheckWorkStatus(Guid id,
        [FromBody] TaskCheckWorkStatusCommand request,
        [FromServices] ICheckWorkStatus checkWorkStatus,
        CancellationToken cancellationToken)
    {
        await checkWorkStatus.CheckAsync(id, request, cancellationToken);
        return Ok();
    }

    [HttpPost]
    [Route("{id}/confirmbooked")]
    public async Task<IActionResult> ConfirmBooked(Guid id,
        [FromBody] TaskConfirmBookedCommand request,
        [FromServices] IConfirmBooked confirmBooked,
        CancellationToken cancellationToken)
    {
        await confirmBooked.ConfirmAsync(id, request, cancellationToken);
        return Ok();
    }
    [HttpPost]
    [Route("{id}/notifycustomer")]
    public async Task<IActionResult> NotifyCustomer(Guid id,
        [FromBody] TaskNotifyCustomerCommand request,
        [FromServices] INotifyCustomer notifyCustomer,
        CancellationToken cancellationToken)
    {
        await notifyCustomer.NotifyAsync(id, request, cancellationToken);
        return Ok();
    }
    [HttpPost]
    [Route("{id}/notifycustomerdelay")]
    public async Task<IActionResult> NotifyCustomerDelay(Guid id,
        [FromBody] TaskNotifyCustomerDelayCommand request,
        [FromServices] INotifyCustomerDelay notifyCustomerDelay,
        CancellationToken cancellationToken)
    {
        await notifyCustomerDelay.NotifyAsync(id, request, cancellationToken);
        return Ok();
    }

    [HttpPost]
    [Route("{id}/checkarrival")]
    public async Task<IActionResult> CheckArrival(Guid id,
        [FromBody] TaskCheckArrivalCommand request,
        [FromServices] ICheckArrival checkArrival,
        CancellationToken cancellationToken)
    {
        await checkArrival.CheckAsync(id, request, cancellationToken);
        return Ok();
    }

    [HttpPost]
    [Route("{id}/chasedriver")]
    public async Task<IActionResult> ChaseDriver(Guid id,
        [FromBody] TaskChaseDriverCommand request,
        [FromServices] IChaseDriver chaseDriver,
        CancellationToken cancellationToken)
    {
        await chaseDriver.ChaseAsync(id, request, cancellationToken);
        return Ok();
    }

    [HttpGet]
    [Route("{id}/suppliers")]
    public async Task<IActionResult> GetSuppliers(Guid id,
        [FromServices] ISupplierLocator supplierLocator,
        CancellationToken cancellationToken)
    {
        var serviceBooking = await _context.ServiceBookings.FindAsync(id, cancellationToken);
        if (serviceBooking is null)
        {
            return NotFound();
        }

        var distances = await supplierLocator.GetSuppliers(serviceBooking, cancellationToken);

        var suppliers = distances.Select(s => new SupplierLocatorDto(s.Code, s.Name, s.Distance));

        //var vehicle = await _context.Vehicles.FindAsync(serviceBooking.VehicleId, cancellationToken);
        //if (vehicle is null)
        //{
        //    return NotFound();
        //}

        //var suppliers = await _context.Suppliers.AsNoTracking()
        //    //.Where(s => s.Address.Location.Distance(vehicle.Address.Location) > 0)
        //    .OrderBy(s => s.Address.Location.Distance(vehicle.Address.Location))
        //    .Select(s => new SupplierLocatorDto(s.Code, s.Name, s.Address.Location.Distance(vehicle.Address.Location)))
        //    .ToListAsync(cancellationToken);

        return Ok(suppliers);
    }

    [HttpPost]
    public async Task<IActionResult> CreateServiceBooking(
        [FromBody] CreateServiceBookingCommand request,
        [FromServices] IAssignSupplierUseCase assignSupplierUseCase,
        CancellationToken cancellationToken)
    {
        var serviceBooking = await new CreateServiceBooking(_context, assignSupplierUseCase)
            .CreateAsync(request, cancellationToken);

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
            .Include(s => s.Supplier)
            .Include(s=>s.MotEvent)
            .SingleOrDefaultAsync(v => v.Id == id, cancellationToken);

        return serviceBooking is null ? NotFound() : Ok(serviceBooking.ToFullDto());
    }

    [HttpPost]
    [Route("{id}/edit")]
    public async Task<IActionResult> Edit([FromRoute] Guid id,
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
        serviceBooking.ServiceLevel = (Vms.Domain.Entity.ServiceBookingEntity.ServiceLevel)request.ServiceLevel;

        if (serviceBooking.Status == ServiceBookingStatus.None && serviceBooking.IsValid)
        {
            serviceBooking.ChangeStatus(ServiceBookingStatus.Assign);
        }

        return Ok();
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

    public static MotEventShortDto ToShortDto(this MotEvent motEvent) => new (motEvent.Id, motEvent.Due);

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
            (int)serviceBooking.Status,
            (Vms.Web.Shared.ServiceLevel)serviceBooking.ServiceLevel,
            serviceBooking.Supplier?.ToSupplierShortDto(),
            serviceBooking.MotEvent?.ToShortDto()
        );
    }

    public static SupplierShortDto ToSupplierShortDto(this Supplier supplier)
        => new(supplier.Code, supplier.Name);
}