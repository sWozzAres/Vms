using Utopia.Api.Application.Services;
using Vms.Application.Commands.ServiceBookingUseCase;
using Vms.Application.Commands.VehicleUseCase;
using Vms.Domain.ServiceBookingProcess;

namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class ServiceBookingController(ILogger<ServiceBookingController> logger, VmsDbContext context, ITimeService timeService) : ControllerBase
{
    readonly ILogger<ServiceBookingController> _logger = logger ?? throw new ArgumentNullException(nameof(context));
    readonly VmsDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    #region Follow
    [HttpPost]
    [Route("{id}/follow")]
    public async Task<IActionResult> Follow(Guid id,
        [FromServices] FollowServiceBooking follow,
        CancellationToken cancellationToken)
    {
        await follow.FollowAsync(id, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpDelete]
    [Route("{id}/follow")]
    public async Task<IActionResult> Unfollow(Guid id,
        [FromServices] UnfollowServiceBooking unfollow,
        CancellationToken cancellationToken)
    {
        if (!await unfollow.UnfollowAsync(id, cancellationToken))
            return NotFound();

        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }
    #endregion
    #region Locking
    [HttpPost]
    [Route("{id}/lock")]
    public async Task<IActionResult> Lock(Guid id,
        [FromServices] IUserProvider userProvider,
        CancellationToken cancellationToken)
    {
        var lck = ServiceBookingLock.Create(id, userProvider.UserId, userProvider.UserName, timeService.Now);
        _context.ServiceBookingLocks.Add(lck);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            return Ok(new LockDto(lck.Id));
        }
        catch (DbUpdateException dbe) when (dbe.InnerException is SqlException se && se.Message.Contains("IX_ServiceBookingLock_ServiceBookingId"))
        {
            return Forbid();
        }
    }

    [HttpPost]
    [Route("{id}/lock/{lockId}/refresh")]
    public async Task<IActionResult> RefreshLock(Guid id, Guid lockId,
        CancellationToken cancellationToken)
    {
        var lck = await _context.ServiceBookingLocks
            .SingleOrDefaultAsync(x => x.Id == lockId && x.ServiceBookingId == id, cancellationToken);
        if (lck is null)
        {
            return NotFound();
        }

        lck.Granted = timeService.Now;

        await _context.SaveChangesAsync(cancellationToken);

        return Ok();
    }

    [HttpDelete]
    [Route("{id}/lock/{lockId}")]
    public async Task<IActionResult> Unlock(Guid id,
        Guid lockId,
        CancellationToken cancellationToken)
    {
        var serviceBooking = await _context.ServiceBookings.FindAsync(new object[] { id }, cancellationToken);
        if (serviceBooking is null)
        {
            return NotFound();
        }

        var lck = new ServiceBookingLock() { Id = lockId };
        _context.ServiceBookingLocks.Remove(lck);

        try
        {
            await _context.SaveChangesAsync(cancellationToken);

            return Ok();
        }
        catch (DbUpdateConcurrencyException)
        {
            return NotFound();
        }

    }
    #endregion
    #region Tasks
    [HttpPost]
    [Route("{id}/assignsupplier")]
    public async Task<IActionResult> AssignSupplier(Guid id,
        [FromBody] TaskAssignSupplierCommand request,
        [FromServices] AssignSupplier command,
        CancellationToken cancellationToken)
    {
        await command.AssignAsync(id, request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpPost]
    [Route("{id}/unbooksupplier")]
    public async Task<IActionResult> UnbookSupplier(Guid id,
        [FromBody] TaskUnbookSupplierCommand request,
        [FromServices] UnbookSupplier command,
        CancellationToken cancellationToken)
    {
        await command.UnbookAsync(id, request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpPost]
    [Route("{id}/booksupplier")]
    public async Task<IActionResult> BookSupplier(Guid id,
        [FromBody] TaskBookSupplierCommand request,
        [FromServices] BookSupplier command,
        CancellationToken cancellationToken)
    {
        await command.BookAsync(id, request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpPost]
    [Route("{id}/checkworkstatus")]
    public async Task<IActionResult> CheckWorkStatus(Guid id,
        [FromBody] TaskCheckWorkStatusCommand request,
        [FromServices] CheckWorkStatus command,
        CancellationToken cancellationToken)
    {
        await command.CheckAsync(id, request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpPost]
    [Route("{id}/confirmbooked")]
    public async Task<IActionResult> ConfirmBooked(Guid id,
        [FromBody] TaskConfirmBookedCommand request,
        [FromServices] ConfirmBooked command,
        CancellationToken cancellationToken)
    {
        await command.ConfirmAsync(id, request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }
    [HttpPost]
    [Route("{id}/notifycustomer")]
    public async Task<IActionResult> NotifyCustomer(Guid id,
        [FromBody] TaskNotifyCustomerCommand request,
        [FromServices] NotifyCustomer command,
        CancellationToken cancellationToken)
    {
        await command.NotifyAsync(id, request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }
    [HttpPost]
    [Route("{id}/notifycustomerdelay")]
    public async Task<IActionResult> NotifyCustomerDelay(Guid id,
        [FromBody] TaskNotifyCustomerDelayCommand request,
        [FromServices] NotifyCustomerDelay command,
        CancellationToken cancellationToken)
    {
        await command.NotifyAsync(id, request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpPost]
    [Route("{id}/checkarrival")]
    public async Task<IActionResult> CheckArrival(Guid id,
        [FromBody] TaskCheckArrivalCommand request,
        [FromServices] CheckArrival command,
        CancellationToken cancellationToken)
    {
        await command.CheckAsync(id, request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpPost]
    [Route("{id}/chasedriver")]
    public async Task<IActionResult> ChaseDriver(Guid id,
        [FromBody] TaskChaseDriverCommand request,
        [FromServices] ChaseDriver command,
        CancellationToken cancellationToken)
    {
        await command.ChaseAsync(id, request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpPost]
    [Route("{id}/rebookdriver")]
    public async Task<IActionResult> RebookDriver(Guid id,
        [FromBody] TaskRebookDriverCommand request,
        [FromServices] RebookDriver command,
        CancellationToken cancellationToken)
    {
        await command.RebookAsync(id, request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Ok();
    }
    #endregion
    #region Activity
    [HttpGet]
    [Route("{id}/activity")]
    public async Task<IActionResult> GetActivities(Guid id,
        [FromServices] DocumentQueries queries,
        CancellationToken cancellationToken)
    => Ok(await queries.GetActivities(id, cancellationToken));

    [HttpPost]
    [Route("{id}/activity")]
    public async Task<IActionResult> PostNote(Guid id,
        [FromBody] AddNoteDto request,
        [FromServices] AddNoteServiceBooking addNote,
        CancellationToken cancellationToken)
    {
        var entry = await addNote.Add(id, request, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetActivity), new { id, activityId = entry.Id }, entry);
    }

    [HttpGet]
    [Route("{id}/activity/{activityId}")]
    public async Task<IActionResult> GetActivity(Guid id, Guid activityId,
        [FromServices] ServiceBookingQueries queries,
        CancellationToken cancellationToken)
    {
        var activityLog = await queries.GetActivity(id, activityId, cancellationToken);
        return activityLog is null ? NotFound() : Ok(activityLog);
    }
    #endregion
    #region CRUD
    [HttpPost]
    public async Task<IActionResult> CreateServiceBooking(
        [FromBody] CreateServiceBookingCommand request,
        //[FromServices] IAutomaticallyAssignSupplierUseCase assignSupplierUseCase,
        [FromServices] CreateServiceBooking command,
        CancellationToken cancellationToken)
    {
        //ModelState.AddModelError("ServiceLevel", "An error");
        //return UnprocessableEntity(ModelState);

        //throw new VmsDomainException("Domain Error");

        //int tries = 1;
        //while (true)
        //{
        //    try
        //    {
        var result = await _context.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            using var transaction = _context.Database.BeginTransaction();

            var serviceBooking = await command //new CreateServiceBooking(_context, assignSupplierUseCase)
                    .CreateAsync(request, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            await transaction.CommitAsync();

            return serviceBooking;

        });
        return CreatedAtAction("GetServiceBooking", new { id = result.Id }, result.ToDto());
        //}
        //catch (DbUpdateException ex) when (ex.InnerException is SqlException && ex.InnerException.Message.Contains("UQ_ServiceBooking_Ref"))
        //{
        //    if (tries == 3)
        //    {
        //        logger.LogCritical("Failed to create service booking with unique ref.");
        //        throw;
        //    }

        //    tries++;
        //}
        //}
    }
    [HttpPost]
    [Route("{id}/edit")]
    public async Task<IActionResult> Edit([FromRoute] Guid id,
        [FromBody] ServiceBookingDto request,
        [FromServices] EditServiceBooking command,
        CancellationToken cancellationToken)
    {
        if (request.Id != id)
        {
            return BadRequest();
        }

        if (await command.EditAsync(id, request, cancellationToken))
            await _context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
    //[HttpDelete]
    //[Route("{id}/activity/{activityId}")]
    //public async Task<IActionResult> DeleteActivity(Guid id, Guid activityId,
    //    CancellationToken cancellationToken)
    //{
    //    var serviceBooking = await _context.ServiceBookings.AsNoTracking()
    //            .SingleOrDefaultAsync(v => v.Id == id, cancellationToken);

    //    if (serviceBooking is null)
    //    {
    //        return NotFound();
    //    }

    //    //ActivityLog al = new ActivityLog(activityId)
    //    return NoContent();
    //}
    #endregion

    [HttpGet]
    [Route("{id}/suppliers")]
    public async Task<IActionResult> GetSuppliers(Guid id, string? filter,
        [FromServices] SupplierLocator supplierLocator,
        CancellationToken cancellationToken)
    {
        var serviceBooking = await _context.ServiceBookings.FindAsync(new object[] { id }, cancellationToken);
        if (serviceBooking is null)
        {
            return NotFound();
        }

        var distances = await supplierLocator.GetSuppliers(serviceBooking, filter, cancellationToken);
        return Ok(distances.Select(s => new SupplierLocatorDto(s.Code, s.Name, s.Distance, s.RefusalCode, s.RefusalName)));
    }

    [HttpGet]
    public async Task<IActionResult> GetServiceBookings(
        ServiceBookingListOptions list, int start, int take,
        ServiceBookingQueries queries,
        CancellationToken cancellationToken)
    {
        var (totalCount, result) = await queries.GetServiceBookings(list, start, take, cancellationToken);
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

    [HttpGet]
    [Route("{id}")]
    [AcceptHeader("application/vnd.full")]
    public async Task<IActionResult> GetServiceBookingFull(Guid id,
        ServiceBookingQueries queries,
        [FromServices] IRecentViewLogger<VmsDbContext> recentViewLogger,
        CancellationToken cancellationToken)
    {
        var serviceBooking = await queries.GetServiceBookingFull(id, cancellationToken);
        if (serviceBooking is null)
        {
            return NotFound();
        }

        await recentViewLogger.LogAsync(serviceBooking.Id);

        await context.SaveChangesAsync(cancellationToken);

        return Ok(serviceBooking);
    }

    [HttpGet]
    [Route("vehicle/{id}")]
    [AcceptHeader("application/vnd.full")]
    [ProducesResponseType(typeof(VehicleFullDto), StatusCodes.Status200OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetServiceBookingsFullByVehicle(Guid id,
        ServiceBookingQueries queries,
        CancellationToken cancellationToken)
    {
        var serviceBookings = await queries.GetServiceBookingsFullByVehicle(id, cancellationToken);
        return Ok(serviceBookings.ToList());
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
}