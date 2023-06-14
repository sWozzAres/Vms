using Vms.Domain.Services;
using System.Security.Claims;

namespace Vms.Web.Server.Services;

//public class UserProvider : IUserProvider
//{
//    readonly IHttpContextAccessor Context;

//    public UserProvider(IHttpContextAccessor context)
//        => Context = context;

//    public string UserId 
//        => Context.HttpContext?.User.UserId() ?? string.Empty;
    
//    public string TenantId 
//        => Context.HttpContext?.User.TenantId() ?? string.Empty;
//}

public static class ClaimsPrincipalExtensions
{
    public static string? UserId(this ClaimsPrincipal principal) 
        => principal.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
    
    public static string? TenantId(this ClaimsPrincipal principal) 
        => principal.Claims.FirstOrDefault(x => x.Type == "tenantid")?.Value;
}

//public class UserProvider2 : IUserProvider
//{
//    readonly ClaimsPrincipal Principal;

//    public UserProvider2(ClaimsPrincipal principal)
//        => Principal = principal;

//    public string UserId => Principal.Claims.FirstOrDefault(x => x.Type == "sub")?.Value ?? string.Empty;

//    public string TenantId => Principal.Claims.FirstOrDefault(x => x.Type == "tenantid")?.Value ?? string.Empty;
//}

public class UserProvider : IUserProvider
{
    string _userId;
    public string UserId => _userId;
    string _tenantId;
    public string TenantId => _tenantId;
    public UserProvider(IHttpContextAccessor context)
    {
        _userId = context.HttpContext?.User.UserId() ?? throw new InvalidOperationException("Failed to retrieve user id.");
        _tenantId = context.HttpContext?.User.TenantId() ?? throw new InvalidOperationException("Failed to retrieve tenant id.");
    }
}