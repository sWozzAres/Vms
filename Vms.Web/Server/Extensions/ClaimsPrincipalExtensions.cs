using System.Security.Claims;

namespace Vms.Web.Server.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string? UserId(this ClaimsPrincipal principal)
        => principal.Claims
            .FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier" || x.Type == "sub")?.Value;

    public static string? TenantId(this ClaimsPrincipal principal)
        => principal.Claims.FirstOrDefault(x => x.Type == "tenantid")?.Value;

    public static string? Name(this ClaimsPrincipal principal)
        => principal.Claims.FirstOrDefault(x => x.Type == "name")?.Value;

    public static string? Email(this ClaimsPrincipal principal)
        => principal.Claims
            .FirstOrDefault(x => x.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress" || x.Type == "email")?.Value;
}
