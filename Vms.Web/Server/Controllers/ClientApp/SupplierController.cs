﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utopia.Blazor.Application.Shared;
using Vms.Application.Queries;
using Vms.Domain.Core;
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

    [HttpGet]
    public async Task<IActionResult> Get(
        SupplierListOptions list, int start, int take,
        [FromServices] ISupplierQueries queries,
        CancellationToken cancellationToken)
    {
        var (totalCount, result) = await queries.GetSuppliers(list, start, take, cancellationToken);
        return Ok(new ListResult<SupplierListDto>(totalCount, result));
    }
}