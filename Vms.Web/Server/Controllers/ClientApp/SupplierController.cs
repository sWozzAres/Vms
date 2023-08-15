using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Utopia.Blazor.Application.Shared;
using Vms.Application.Commands.SupplierUseCase;
using Vms.Application.Commands.VehicleUseCase;
using Vms.Application.Queries;
using Vms.Application.Services;
using Vms.Domain.Infrastructure;
using Vms.Web.Shared;

namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class SupplierController(VmsDbContext context) : ControllerBase
{
    readonly VmsDbContext _context = context;

    #region CRUD
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
    #endregion
    [HttpGet]
    public async Task<IActionResult> Get(
        SupplierListOptions list, int start, int take,
        [FromServices] ISupplierQueries queries,
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
        [FromServices] ISupplierQueries queries,
        [FromServices] IRecentViewLogger recentViewLogger,
        CancellationToken cancellationToken)
    {
        var supplier = await queries.GetSupplierFull(code, cancellationToken);
        if (supplier is null)
        {
            return NotFound();
        }

        //await recentViewLogger.LogAsync(supplier.Id);

        await context.SaveChangesAsync(cancellationToken);

        return Ok(supplier);
    }
}