namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class NetworkController : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        NetworkListOptions list, int start, int take,
        [FromServices] NetworkQueries queries,
        CancellationToken cancellationToken)
    {
        var (totalCount, result) = await queries.GetNetworks(list, start, take, cancellationToken);
        return Ok(new ListResult<NetworkListDto>(totalCount, result));
    }
}