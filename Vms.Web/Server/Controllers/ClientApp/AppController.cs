using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Vms.Domain.Entity;
using Vms.Domain.Infrastructure;
using Vms.Domain.Services;

namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class AppController : ControllerBase
{
    readonly ILogger<CompanyController> _logger;
    readonly VmsDbContext _context;

    public AppController(ILogger<CompanyController> logger, VmsDbContext context)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterLogin(IUserProvider userProvider, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(userProvider.UserId, cancellationToken);
        if (user == null)
        {
            user = new Domain.Entity.User()
            {
                UserId = userProvider.UserId,
                UserName = userProvider.UserName,
            };

            _context.Users.Add(user);
        }

        // update username
        if (user.UserName !=  userProvider.UserName)
        {
            user.UserName = userProvider.UserName;
        }

        var login = new Login()
        {
            UserId = userProvider.UserId,
            LoginTime = DateTime.Now,
        };

        _context.Logins.Add(login);

        await _context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
