using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vms.Blazor.Shared.Admin;
using Vms.Domain.Infrastructure;

namespace Vms.Blazor.Server.Controllers.Admin;

[Authorize(Policy = "AdminPolicy")]
[Route("api/admin/[controller]")]
[ApiController]
[Produces("application/json")]
public class CompanyController : Controller
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CompanyListModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CompanyListModel>>> GetCompanies(
        //[FromServices] IAccountQueries _queries,
        [FromServices] VmsDbContext context,
        CancellationToken cancellationToken)
    {
        var result = await context.Companies
            .Select(x=>new { x.Code, x.Name })
            .ToListAsync(cancellationToken);

        return Ok(result.Select(x=>new CompanyListModel(x.Code, x.Name)));
    }

    [HttpGet("{code}")]
    [ProducesResponseType(typeof(CompanyModel), StatusCodes.Status200OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<CompanyModel>> GetCompany(int code,
        [FromServices] VmsDbContext context,
        CancellationToken cancellationToken)
    {
        var company = await context.Companies.FindAsync(code, cancellationToken);
        if (company == null)
        {
            return NotFound();
        }

        return new CompanyModel(company.Code, company.Name);
    }
}
