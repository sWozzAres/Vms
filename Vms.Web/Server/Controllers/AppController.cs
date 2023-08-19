using Catalog.Api.Domain.Infrastructure;
using Utopia.Api.Application.Commands;
using Utopia.Api.Domain.Infrastructure;
using Utopia.Api.Domain.System;
using Vms.Application.Queries;

namespace Vms.Web.Server.Controllers;

[ApiController]
[Route("ClientApp/api/[controller]")]
[Authorize(Policy = "ClientPolicy")]
[Produces("application/json")]
public class AppController(ILogger<AppController> logger) : ControllerBase
{
    [HttpGet]
    [Route("notifications")]
    public async Task<IActionResult> GetActivityNotificatons(
        [FromServices] IUtopiaQueries<VmsDbContext> queries,
        CancellationToken cancellationToken)
    {
        return Ok((await queries.GetActivityNotifications(cancellationToken)).ToList());
    }
    [HttpPost]
    [Route("notifications/{id}/markasread")]
    public async Task<IActionResult> MarkAsRead(long id,
        [FromServices] VmsDbContext context,
        [FromServices] IMarkActivityNotificationAsRead<VmsDbContext> markActivityNotificationAsRead,
        CancellationToken cancellationToken)
    {
        await markActivityNotificationAsRead.Mark(id, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return Ok();
    }
    [HttpGet]
    [Route("users/{code}")]
    public async Task<IActionResult> GetUsersForTenant(string code,
        [FromServices] VmsDbContext context,
        IUserProvider userProvider,
        CancellationToken cancellationToken)
    {
        if (!userProvider.HasAccessToTenant(code))
        {
            logger.LogError("User '{userId}' has no access to tenant '{companyCode}'.", userProvider.UserId, code);
            throw new InvalidOperationException("User has no access to tenant.");
        }

        var users = await context.Users.Where(u => u.TenantId == "*" || u.TenantId == code)
                .Select(u => new UserDto(u.UserId, u.UserName))
                .ToListAsync(cancellationToken);

        return Ok(users);
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterLogin(IUserProvider userProvider,
        [FromServices] VmsDbContext context1,
        [FromServices] CatalogDbContext context2,
        CancellationToken cancellationToken)
    {
        await UpdateUserInContext(context1, userProvider, cancellationToken);
        await UpdateUserInContext(context2, userProvider, cancellationToken);

        return Ok();
    }

    async Task UpdateUserInContext(ISystemContext context, IUserProvider userProvider, CancellationToken cancellationToken)
    {
        var user = await context.Users.FindAsync(new object[] { userProvider.UserId }, cancellationToken);
        if (user is null)
        {
            user = new User(userProvider.UserId, userProvider.UserName, userProvider.TenantId, userProvider.EmailAddress);
            logger.LogInformation("Creating information record for user {user} in context {context}.", user, context.GetType());
            context.Users.Add(user);
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

            if (user.EmailAddress != userProvider.EmailAddress)
            {
                user.EmailAddress = userProvider.EmailAddress;
            }
        }

        context.Logins.Add(new Login(userProvider.UserId, DateTime.Now));

        await context.SaveChangesAsync(cancellationToken);
    }
}
