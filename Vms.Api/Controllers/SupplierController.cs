using Vms.Application.Commands.SupplierUseCase;
using Vms.Application.Extensions;

namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class SupplierController(VmsDbContext context) : ControllerBase
{
    #region Activity
    [HttpGet]
    [Route("{id}/activity")]
    public async Task<IActionResult> GetActivities(Guid id,
        [FromServices] DocumentQueries documentQueries,
        CancellationToken cancellationToken)
    => Ok(await documentQueries.GetActivities(id, cancellationToken));

    [HttpPost]
    [Route("{id}/activity")]
    public async Task<IActionResult> PostNote(Guid id,
        [FromBody] AddNoteDto request,
        [FromServices] IAddNoteSupplier addNote,
        CancellationToken cancellationToken)
    {
        var entry = await addNote.AddAsync(id, request, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return CreatedAtAction(nameof(GetActivity), new { id, activityId = entry.Id }, entry);
    }

    [HttpGet]
    [Route("{id}/activity/{activityId}")]
    public async Task<IActionResult> GetActivity(Guid id, Guid activityId,
        [FromServices] SupplierQueries queries,
        CancellationToken cancellationToken)
    {
        var activityLog = await queries.GetActivity(id, activityId, cancellationToken);
        return activityLog is null ? NotFound() : Ok(activityLog);
    }
    #endregion
    #region Follow
    [HttpPost]
    [Route("{id}/follow")]
    public async Task<IActionResult> Follow(Guid id,
        [FromServices] IFollowSupplier follow,
        CancellationToken cancellationToken)
    {
        await follow.FollowAsync(id, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return Ok();
    }

    [HttpDelete]
    [Route("{id}/follow")]
    public async Task<IActionResult> Unfollow(Guid id,
        [FromServices] IUnfollowSupplier unfollow,
        CancellationToken cancellationToken)
    {
        if (!await unfollow.UnfollowAsync(id, cancellationToken))
            return NotFound();

        await context.SaveChangesAsync(cancellationToken);
        return Ok();
    }
    #endregion
    #region CRUD
    [HttpPost]
    [Route("")]
    public async Task<IActionResult> CreateSupplier(
        [FromBody] CreateSupplierDto supplierDto,
        [FromServices] ICreateSupplier createSupplier,
        CancellationToken cancellationToken)
    {
        var request = new CreateSupplierRequest(supplierDto.Code, supplierDto.Name,
            new AddressDto("", "", "", "", new GeometryDto(0, 0)), false);

        var supplier = await createSupplier.CreateAsync(request);
        try
        {
            await context.SaveChangesAsync(cancellationToken);

            return CreatedAtAction("GetSupplierFull", new { code = supplier.Code }, supplier.ToDto());
        }
        catch (DbUpdateException dbe) when (dbe.InnerException is SqlException se && se.Message.Contains("PK_Suppliers"))
        {
            throw new VmsDomainException($"The supplier with code '{supplier.Code}' already exists.");
        }
    }
    [HttpPost]
    [Route("{code}/edit")]
    public async Task<IActionResult> Edit([FromRoute] string code,
        [FromBody] SupplierDto request,
        [FromServices] IEditSupplier edit,
        CancellationToken cancellationToken)
    {
        if (request.Code != code)
        {
            return BadRequest();
        }

        await edit.EditAsync(code, request, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
    [HttpGet]
    public async Task<IActionResult> Get(
        SupplierListOptions list, int start, int take,
        [FromServices] SupplierQueries queries,
        CancellationToken cancellationToken)
    {
        var (totalCount, result) = await queries.GetSuppliers(list, start, take, cancellationToken);
        return Ok(new ListResult<SupplierListDto>(totalCount, result));
    }
    [HttpGet]
    [Route("{code}")]
    [AcceptHeader("application/vnd.full")]
    [ProducesResponseType(typeof(SupplierFullDto), StatusCodes.Status200OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetSupplierFull(string code,
        [FromServices] SupplierQueries queries,
        [FromServices] IRecentViewLogger<VmsDbContext> recentViewLogger,
        CancellationToken cancellationToken)
    {
        var supplier = await queries.GetSupplierFull(code, cancellationToken);
        if (supplier is null)
        {
            return NotFound();
        }

        await recentViewLogger.LogAsync(supplier.Id);

        await context.SaveChangesAsync(cancellationToken);

        return Ok(supplier);
    }
    #endregion
}