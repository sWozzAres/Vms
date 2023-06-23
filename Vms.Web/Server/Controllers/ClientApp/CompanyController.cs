﻿using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vms.Web.Shared;
using Vms.Domain.Infrastructure;
using Vms.Domain.Entity;
using Microsoft.VisualBasic;

namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class CompanyController : ControllerBase
{
    private readonly ILogger<CompanyController> _logger;

    public CompanyController(ILogger<CompanyController> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    //[HttpGet]
    //[Route("")]
    //[ProducesResponseType(typeof(IEnumerable<CompanyListModel>), StatusCodes.Status200OK)]
    //public async Task<ActionResult<IEnumerable<CompanyListModel>>> GetCompanies(
    //    int? list, int? start, int? take,
    //    //[FromServices] IAccountQueries _queries,
    //    [FromServices] VmsDbContext context,
    //    CancellationToken cancellationToken)
    //{
    //    var result = await context.Companies
    //        .Skip(start)
    //        .Take(take)
    //        .Select(x => new CompanyListModel(x.Code, x.Name))
    //        .ToListAsync(cancellationToken);

    //    return Ok(result);
    //}

    [HttpGet]
    [Route("{code}")]
    [ProducesResponseType(typeof(CompanyModel), StatusCodes.Status200OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<CompanyModel>> GetCompany(string code,
        [FromServices] VmsDbContext context,
        CancellationToken cancellationToken)
    {
        var company = await context.Companies.FindAsync(new[] { code }, cancellationToken);
        if (company is null)
        {
            return NotFound();
        }

        return company.ToDto();// new CompanyModel(company.Code, company.Name);
    }

    [HttpPatch]
    [Route("{code}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    //[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    [ProducesResponseType(StatusCodes.Status412PreconditionFailed)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status428PreconditionRequired)]
    public async Task<IActionResult> PatchAsync([FromRoute] string code,
        [FromServices] VmsDbContext context,
        [FromBody] CompanyModel request,
        CancellationToken cancellationToken)
    {
        var company = await context.Companies.FindAsync(new[] { code }, cancellationToken);
        if (company is null)
        {
            return NotFound();
        }

        company.Name = request.Name;

        if (context.Entry(company).State == EntityState.Modified)
        {
            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                _logger.LogInformation("Concurrency violation while modifying company code '{code}'.", company.Code);

                //if (rowVersion is not null)
                //    return StatusCode(StatusCodes.Status412PreconditionFailed);

                //TODO retry?
                return Problem();
            }

            //HttpContext.SetETag(product.RowVersion);

            return Ok(company.ToDto());
        }

        // return NoContent to signify no changes
        return NoContent();
    }
}

public static partial class DomainExtensions
{
    public static CompanyModel ToDto(this Company company)
        => new()
        {
            Code = company.Code,
            Name = company.Name,
        };
}