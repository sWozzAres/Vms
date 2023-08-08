using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vms.Domain.Core;
using Vms.Domain.Infrastructure;
using Vms.Web.Shared;

namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class CompanyController(ILogger<CompanyController> logger, VmsDbContext context) : ControllerBase
{
    readonly ILogger<CompanyController> _logger = logger;
    readonly VmsDbContext _context = context;
    #region Customers
    [HttpGet]
    [Route("{companyCode}/customers/{code}")]
    [AcceptHeader("application/vnd.short")]
    [ProducesResponseType(typeof(CustomerShortDto), StatusCodes.Status200OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetCustomerShort(string companyCode, string code, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers.AsNoTracking()
            .SingleOrDefaultAsync(d => d.CompanyCode == companyCode && d.Code == code, cancellationToken);

        if (customer is null)
        {
            return NotFound();
        }

        return Ok(new CustomerShortDto(customer.CompanyCode, customer.Code, customer.Name));
    }
    [HttpGet]
    [Route("{code}/customers")]
    [AcceptHeader("application/vnd.short")]
    [ProducesResponseType(typeof(CustomerShortDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCustomersShort(string code, string filter, CancellationToken cancellationToken)
        => Ok(await _context.Customers.AsNoTracking()
                .Where(d => d.CompanyCode == code && d.Name.StartsWith(filter))
                .Select(d => new CustomerShortDto(d.CompanyCode, d.Code, d.Name))
                .ToListAsync(cancellationToken));
    #endregion
    #region Fleets
    [HttpGet]
    [Route("{companyCode}/fleet/{code}")]
    [AcceptHeader("application/vnd.short")]
    [ProducesResponseType(typeof(FleetShortDto), StatusCodes.Status200OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> GetFleetShort(string companyCode, string code, CancellationToken cancellationToken)
    {
        var fleet = await _context.Fleets.AsNoTracking()
            .SingleOrDefaultAsync(d => d.CompanyCode == companyCode && d.Code == code, cancellationToken);

        if (fleet is null)
        {
            return NotFound();
        }

        return Ok(new FleetShortDto(fleet.CompanyCode, fleet.Code, fleet.Name));
    }
    [HttpGet]
    [Route("{code}/fleets")]
    [AcceptHeader("application/vnd.short")]
    [ProducesResponseType(typeof(FleetShortDto), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetFleetsShort(string code, string filter, CancellationToken cancellationToken)
        => Ok(await _context.Fleets.AsNoTracking()
                .Where(f => f.CompanyCode == code && f.Name.StartsWith(filter))
                .Select(f => f.ToShortDto())
                .ToListAsync(cancellationToken));
    #endregion
    #region Reasons
    [HttpGet]
    [Route("{companyCode}/confirmbookedrefusalreasons")]
    public async Task<IActionResult> GetConfirmBookedRefusalReasons(string companyCode, CancellationToken cancellationToken)
        => Ok(await _context.ConfirmBookedRefusalReasons
            .Where(r => r.CompanyCode == companyCode)
            .Select(r => new ConfirmBookedRefusalReasonDto(r.Code, r.Name))
            .ToListAsync(cancellationToken));

    [HttpGet]
    [Route("{companyCode}/refusalreasons")]
    public async Task<IActionResult> GetRefusalReasons(string companyCode, CancellationToken cancellationToken)
        => Ok(await _context.RefusalReasons
            .Where(r => r.CompanyCode == companyCode)
            .Select(r => new RefusalReasonDto(r.Code, r.Name))
            .ToListAsync(cancellationToken));

    [HttpGet]
    [Route("{companyCode}/nonarrivalreasons")]
    public async Task<IActionResult> GetNonArrivalReasons(string companyCode, CancellationToken cancellationToken)
        => Ok(await _context.NonArrivalReasons
            .Where(r => r.CompanyCode == companyCode)
            .Select(r => new NonArrivalReasonDto(r.Code, r.Name))
            .ToListAsync(cancellationToken));
    [HttpGet]
    [Route("{companyCode}/notcompletereasons")]
    public async Task<IActionResult> GetNotCompleteReasons(string companyCode, CancellationToken cancellationToken)
        => Ok(await _context.NotCompleteReasons
            .Where(r => r.CompanyCode == companyCode)
            .Select(r => new NotCompleteReasonDto(r.Code, r.Name))
            .ToListAsync(cancellationToken));

    [HttpGet]
    [Route("{companyCode}/reschedulereasons")]
    public async Task<IActionResult> GetRescheduleReasons(string companyCode, CancellationToken cancellationToken)
        => Ok(await _context.RescheduleReasons
            .Where(r => r.CompanyCode == companyCode)
            .Select(r => new RescheduleReasonDto(r.Code, r.Name))
            .ToListAsync(cancellationToken));
    #endregion

    [HttpGet]
    [AcceptHeader("application/vnd.short")]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var result = await _context.Companies
            .Select(c => new CompanyListModel(c.Code, c.Name))
            .ToListAsync(cancellationToken);

        return Ok(result);
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
                _logger.LogInformation("Concurrency violation while modifying company, code '{code}'.", company.Code);

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