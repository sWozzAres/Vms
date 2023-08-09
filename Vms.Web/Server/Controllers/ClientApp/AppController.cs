using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vms.Domain.Infrastructure;
using Vms.Domain.Infrastructure.Services;
using Vms.Domain.System;
using Vms.Web.Shared;

namespace Vms.Web.Server.Controllers.ClientApp;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class AppController(ILogger<AppController> logger, VmsDbContext context) : ControllerBase
{
    readonly ILogger<AppController> _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    readonly VmsDbContext _context = context ?? throw new ArgumentNullException(nameof(context));

    [HttpGet]
    [Route("users/{code}")]
    public async Task<IActionResult> GetUsersForCompany(string code,
        IUserProvider userProvider,
        CancellationToken cancellationToken)
    {
        if (!userProvider.HasAccessToTenant(code))
        {
            logger.LogError("User '{userId}' has no access to tenant '{companyCode}'.", userProvider.UserId, code);
            throw new InvalidOperationException("User has no access to tenant.");
        }

        var users = await _context.Users.Where(u => u.TenantId == "*" || u.TenantId == code)
                .Select(u => new UserDto(u.UserId, u.UserName))
                .ToListAsync(cancellationToken);

        return Ok(users);

    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterLogin(IUserProvider userProvider, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FindAsync(new object[] { userProvider.UserId }, cancellationToken);
        if (user is null)
        {
            user = new User(userProvider.UserId, userProvider.UserName, userProvider.TenantId);
            _logger.LogInformation("Creating information record for user {user}.", user);
            _context.Users.Add(user);
        }
        else
        {
            // update username / tenantid
            if (user.UserName != userProvider.UserName)
            {
                user.UserName = userProvider.UserName;
            }

            if (user.TenantId != userProvider.TenantId)
            {
                user.TenantId = userProvider.TenantId;
            }
        }

        _context.Logins.Add(new Login(userProvider.UserId, DateTime.Now));

        await _context.SaveChangesAsync(cancellationToken);

        return Ok();
    }
}
