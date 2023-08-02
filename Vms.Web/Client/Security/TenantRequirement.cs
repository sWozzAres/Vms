using Microsoft.AspNetCore.Authorization;

namespace Vms.Web.Client.Security;

public class TenantRequirement(string tenantId) : IAuthorizationRequirement
{
    public string TenantId { get; } = tenantId;
}
