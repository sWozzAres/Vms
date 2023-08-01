using System.Net;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Vms.Application.Services;
using Vms.Application.UseCase;
using Vms.Application.UseCase.ServiceBookingUseCase;
using Vms.Domain.Entity;
using Vms.Domain.Entity.ServiceBookingEntity;
using Vms.Domain.Infrastructure;
using Vms.Domain.Services;
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
    [Route("{id}/follow")]
    public async Task<IActionResult> Follow(Guid id,
        [FromServices] IFollow follow,
        CancellationToken cancellationToken)
    {
        await follow.FollowAsync(id, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpDelete]
    [Route("{id}/follow")]
    public async Task<IActionResult> Unfollow(Guid id,
        IUserProvider userProvider,
        CancellationToken cancellationToken)
    {
        var follow = await _context.Followers
            .SingleOrDefaultAsync(f => f.UserId == userProvider.UserId && f.DocumentId == id);

        if (follow is null)
        {
            return NotFound();
        }

        _context.Followers.Remove(follow);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpPost]
    [Route("{id}/lock")]
    public async Task<IActionResult> Lock(Guid id,
        [FromServices] IUserProvider userProvider,
        CancellationToken cancellationToken)
    {
        var lck = ServiceBookingLock.Create(id, userProvider.UserId, userProvider.UserName);
        _context.ServiceBookingLocks.Add(lck);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return Ok(new LockDto(lck.Id));
        }
        catch (DbUpdateException dbe) when (dbe.InnerException is SqlException se && se.Message.Contains("IX_ServiceBookingLock_ServiceBookingId"))
        {
            //if (dbe.InnerException is SqlException se) {
            //    logger.LogInformation("{errorCode}", se.ErrorCode);
            //    logger.LogInformation("{msg}", se.Message);
            //};
            //throw;
            return Forbid();
        }
    }

    [HttpPost]
    [Route("{id}/lock/{lockId}/refresh")]
    public async Task<IActionResult> RefreshLock(Guid id, Guid lockId,
        //[FromServices] IUserProvider userProvider,
        CancellationToken cancellationToken)
    {
        var lck = await _context.ServiceBookingLocks
            .SingleOrDefaultAsync(x => x.Id == lockId && x.ServiceBookingId == id, cancellationToken);
        if (lck is null)
        {
            return NotFound();
        }

        lck.Granted = DateTime.Now;

        await _context.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    [HttpDelete]
    [Route("{id}/lock/{lockId}")]
    public async Task<IActionResult> Unlock(Guid id,
        Guid lockId,
        CancellationToken cancellationToken)
    {
        var lck = new ServiceBookingLock() { Id = lockId };
        _context.ServiceBookingLocks.Remove(lck);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            return Ok();
        }
        catch (DbUpdateConcurrencyException dbu)
        {
            return NotFound();
        }

    }

    [HttpPost]
    [Route("{id}/assignsupplier")]
    public async Task<IActionResult> AssignSupplier(Guid id,
        [FromBody] TaskAssignSupplierCommand request,
        [FromServices] IAssignSupplierUseCase assignSupplierUseCase,
        CancellationToken cancellationToken)
    {
        await assignSupplierUseCase.AssignAsync(id, request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpPost]
    [Route("{id}/unbooksupplier")]
    public async Task<IActionResult> UnbookSupplier(Guid id,
        [FromBody] TaskUnbookSupplierCommand request,
        [FromServices] IUnbookSupplier unbookSupplier,
        CancellationToken cancellationToken)
    {
        await unbookSupplier.UnbookAsync(id, request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
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
        await _context.SaveChangesAsync(cancellationToken);
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
        await _context.SaveChangesAsync(cancellationToken);
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
        await _context.SaveChangesAsync(cancellationToken);
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
        await _context.SaveChangesAsync(cancellationToken);
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
        await _context.SaveChangesAsync(cancellationToken);
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
        await _context.SaveChangesAsync(cancellationToken);
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
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpPost]
    [Route("{id}/rebookdriver")]
    public async Task<IActionResult> RebookDriver(Guid id,
        [FromBody] TaskRebookDriverCommand request,
        [FromServices] IRebookDriver rebookDriver,
        CancellationToken cancellationToken)
    {
        await rebookDriver.RebookAsync(id, request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
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
        //[FromServices] IAutomaticallyAssignSupplierUseCase assignSupplierUseCase,
        [FromServices] ICreateServiceBooking createServiceBooking,
        CancellationToken cancellationToken)
    {
        var serviceBooking = await createServiceBooking //new CreateServiceBooking(_context, assignSupplierUseCase)
            .CreateAsync(request, cancellationToken);

        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction("GetServiceBooking", new { id = serviceBooking.Id }, serviceBooking.ToDto());
    }

    [HttpGet]
    [Route("{id}/activity")]
    public async Task<IActionResult> GetActivities(Guid id, CancellationToken cancellationToken)
    {
        var a = await _context.ActivityLog.AsNoTracking()
            .Where(l => l.DocumentId == id)
            .OrderBy(l => l.EntryDate)
            .Select(l => l.ToDto())
            .ToListAsync(cancellationToken);

        return Ok(a);
    }
    [HttpPost]
    [Route("{id}/activity")]
    public async Task<IActionResult> PostNote(Guid id,
        [FromBody] AddNoteDto request,
        [FromServices] IUserProvider userProvider,
        CancellationToken cancellationToken)
    {
        var entry = new ActivityLog(id, request.Text, userProvider.UserId, userProvider.UserName);
        _context.ActivityLog.Add(entry);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetActivity), new { id, aid = entry.Id }, entry.ToDto());
    }

    [HttpGet]
    [Route("{id}/activity/{aid}")]
    public async Task<IActionResult> GetActivity(Guid id, Guid aid,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();

        //TODO aid
        var activityLog = await (from sb in _context.ServiceBookings
                                 join ac in _context.ActivityLog on sb.Id equals ac.DocumentId
                                 where sb.Id == id
                                 select ac).SingleOrDefaultAsync(cancellationToken);


        return activityLog is null ? NotFound() : Ok(activityLog.ToDto());
    }

    [HttpGet]
    public async Task<IActionResult> GetServiceBookings(int list, int start, int take, 
        CancellationToken cancellationToken)
    {
        int totalCount = await context.ServiceBookings.CountAsync(cancellationToken);

        var result = await context.ServiceBookings
            .Include(s=>s.Vehicle)
            .Skip(start)
            .Take(take)
            .Select(x => new ServiceBookingListDto() { 
                Id = x.Id, 
                VehicleId = x.VehicleId, 
                Vrm = x.Vehicle.Vrm,
                RescheduleTime = x.RescheduleTime,
                Status = (int)x.Status
            })
            .ToListAsync(cancellationToken);

        return Ok(new ListResult<ServiceBookingListDto>(totalCount, result));
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

    [HttpDelete]
    [Route("{id}/activity/{activityId}")]
    public async Task<IActionResult> DeleteActivity(Guid id, Guid activityId,
        CancellationToken cancellationToken)
    {
        var serviceBooking = await _context.ServiceBookings.AsNoTracking()
                .SingleOrDefaultAsync(v => v.Id == id, cancellationToken);

        if (serviceBooking is null)
        {
            return NotFound();
        }

        //ActivityLog al = new ActivityLog(activityId)
        return NoContent();
    }

    string GetServiceBookingQueryText()
    {
        return """
                SELECT sb.Id, sb.VehicleId, sb.CompanyCode, vv.Vrm, v.Make, v.Model, sb.PreferredDate1, sb.PreferredDate2, sb.PreferredDate3, sb.Status, sb.ServiceLevel,
            	    s.Code 'Supplier_Code', s.Name 'Supplier_Name',
            	    mo.Id 'MotEvent_Id', mo.Due 'MotEvent_Due',
            	    CASE 
            		    WHEN EXISTS (SELECT 1 FROM Followers f WHERE sb.Id = f.DocumentId AND f.UserId = @userId) THEN 1
            		    ELSE 0
            	    END AS IsFollowing,
                    sb.AssignedToUserId
                FROM ServiceBooking sb
                JOIN Vehicle v ON sb.VehicleId = v.Id
                JOIN VehicleVrm vv ON v.Id = vv.VehicleId
                LEFT JOIN Supplier s ON sb.SupplierCode = s.Code
                LEFT JOIN MotEvent mo ON sb.MotEventId = mo.Id
            """;
    }
    [HttpGet]
    [Route("{id}")]
    [AcceptHeader("application/vnd.full")]
    [ProducesResponseType(typeof(VehicleFullDto), StatusCodes.Status200OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetServiceBookingFull(Guid id,
        IUserProvider userProvider,
        CancellationToken cancellationToken)
    {
        var serviceBooking = (await _context.Database.GetDbConnection().QueryAsync<ServiceBookingFullDto>(
            new CommandDefinition($"{GetServiceBookingQueryText()} WHERE sb.Id = @id AND (@tenantId = '*' OR @tenantId = sb.CompanyCode)",
                new { id, userId = userProvider.UserId, tenantId = userProvider.TenantId }, cancellationToken: cancellationToken)))
            .SingleOrDefault();

        return serviceBooking is null ? NotFound() : Ok(serviceBooking);
    }

    [HttpGet]
    [Route("vehicle/{id}")]
    [AcceptHeader("application/vnd.full")]
    [ProducesResponseType(typeof(VehicleFullDto), StatusCodes.Status200OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetServiceBookingFullByVehicle(Guid id,
        IUserProvider userProvider,
        CancellationToken cancellationToken)
    {
        var serviceBookings = await _context.Database.GetDbConnection().QueryAsync<ServiceBookingFullDto>(
            new CommandDefinition($"{GetServiceBookingQueryText()} WHERE @tenantId = '*' OR @tenantId = sb.CompanyCode",
                new { userId = userProvider.UserId, tenantId = userProvider.TenantId }, cancellationToken: cancellationToken));

        return Ok(serviceBookings.ToList());
    }

    [HttpPost]
    [Route("{id}/edit")]
    public async Task<IActionResult> Edit([FromRoute] Guid id,
        [FromBody] ServiceBookingDto request,
        [FromServices] IEdit edit,
        CancellationToken cancellationToken)
    {
        if (request.Id != id)
        {
            return BadRequest();
        }

        await edit.EditAsync(id, request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}

public record ServiceBookingFull(Guid Id, Guid VehicleId, string CompanyCode, string Vrm, string Make, string Model,
        DateOnly? PreferredDate1, DateOnly? PreferredDate2, DateOnly? PreferredDate3, ServiceBookingStatus Status,
        Domain.Entity.ServiceBookingEntity.ServiceLevel ServiceLevel, Supplier? Supplier, MotEvent? MotEvent, Follower? Follower);

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

    public static MotEventShortDto ToShortDto(this MotEvent motEvent) => new(motEvent.Id, motEvent.Due);

    //public static ServiceBookingFullDto ToFullDto(this ServiceBookingFull result) 
    //    => new (
    //        result.Id,
    //        result.VehicleId,
    //        result.CompanyCode,
    //        result.Vrm,
    //        result.Make,
    //        result.Model,
    //        result.PreferredDate1,
    //        result.PreferredDate2,
    //        result.PreferredDate3,
    //        (int) result.Status,
    //        (Vms.Web.Shared.ServiceLevel) result.ServiceLevel,
    //        result.Supplier?.ToSupplierShortDto(),
    //        result.MotEvent?.ToShortDto(),
    //        result.Follower is null
    //    );
    //public static ServiceBookingFullDto ToFullDto(this ServiceBooking serviceBooking)
    //{
    //    return new ServiceBookingFullDto(
    //        serviceBooking.Id,
    //        serviceBooking.VehicleId,
    //        serviceBooking.CompanyCode,
    //        serviceBooking.Vehicle.VehicleVrm.Vrm,
    //        serviceBooking.Vehicle.Make,
    //        serviceBooking.Vehicle.Model,
    //        serviceBooking.PreferredDate1,
    //        serviceBooking.PreferredDate2,
    //        serviceBooking.PreferredDate3,
    //        (int)serviceBooking.Status,
    //        (Vms.Web.Shared.ServiceLevel)serviceBooking.ServiceLevel,
    //        serviceBooking.Supplier?.ToSupplierShortDto(),
    //        serviceBooking.MotEvent?.ToShortDto(),
    //        serviceBooking.Followers.Any()
    //    );
    //}

    public static SupplierShortDto ToSupplierShortDto(this Supplier supplier)
        => new(supplier.Code, supplier.Name);

    public static ActivityLogDto ToDto(this ActivityLog activityLog) => new(activityLog.Id, activityLog.Text, activityLog.EntryDate, activityLog.UserName);
}