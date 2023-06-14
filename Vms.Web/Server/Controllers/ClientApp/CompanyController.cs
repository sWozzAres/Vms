using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vms.Web.Shared;
using Vms.Domain.Infrastructure;

namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class CompanyController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<CompanyListModel>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<CompanyListModel>>> GetCompanies(
        //[FromServices] IAccountQueries _queries,
        [FromServices] VmsDbContext context,
        CancellationToken cancellationToken)
    {
        var result = await context.Companies
            .Select(x => new CompanyListModel(x.Code, x.Name))
            .ToListAsync(cancellationToken);

        return Ok(result);
    }

    [HttpGet("{code}")]
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

        return new CompanyModel(company.Code, company.Name);
    }
}
