using Microsoft.AspNetCore.Authorization;

namespace Vms.Web.Client.Security;

public class TenantHandler : AuthorizationHandler<TenantRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, TenantRequirement requirement)
    {
        if (!context.User.HasClaim(c => c.Type == "tenantid"))
        {
            return Task.CompletedTask;
        }

        var tenantid = context.User.FindFirst(c => c.Type == "tenantid")?.Value ?? throw new InvalidOperationException("Claim not found.");

        if (tenantid == requirement.TenantId)
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}