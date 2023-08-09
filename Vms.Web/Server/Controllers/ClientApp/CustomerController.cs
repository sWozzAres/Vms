﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Utopia.Blazor.Application.Shared;
using Vms.Application.Queries;
using Vms.Domain.Infrastructure;
using Vms.Web.Shared;

namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class CustomerController(VmsDbContext context) : ControllerBase
{
    readonly VmsDbContext _context = context;

    [HttpGet]
    [Route("{filter}")]
    [AcceptHeader("application/vnd.short")]
    [ProducesResponseType(typeof(CustomerShortDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomersShort(string filter, CancellationToken cancellationToken)
        => Ok(await _context.Customers.AsNoTracking()
                .Where(d => d.Name.StartsWith(filter))
                .Select(d => new CustomerShortDto(d.CompanyCode, d.Code, d.Name))
                .ToListAsync(cancellationToken));

    [HttpGet]
    public async Task<IActionResult> Get(
        CustomerListOptions list, int start, int take,
        [FromServices] ICustomerQueries queries,
        CancellationToken cancellationToken)
    {
        var (totalCount, result) = await queries.GetCustomers(list, start, take, cancellationToken);
        return Ok(new ListResult<CustomerListDto>(totalCount, result));
    }

}
