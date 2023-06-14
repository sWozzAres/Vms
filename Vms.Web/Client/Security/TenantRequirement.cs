using Microsoft.AspNetCore.Authorization;

namespace Vms.Web.Client.Security;

public class TenantRequirement : IAuthorizationRequirement
{
    public string TenantId { get; }

    public TenantRequirement(string tenantId) => TenantId = tenantId;
}
