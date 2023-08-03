using Vms.Domain.Services;
using Vms.Web.Server.Extensions;

namespace Vms.Web.Server.Services;

public class UserProvider(IHttpContextAccessor context) : IUserProvider
{
    public static bool InMigration; //TODO make thread safe
    readonly string _userId = context.HttpContext?.User.UserId() ?? "";
    public string UserId => _userId;
    readonly string _tenantId = InMigration ? "*" : context.HttpContext?.User.TenantId() ?? "";
    public string TenantId => _tenantId;
    readonly string _userName = context.HttpContext?.User.Name() ?? "";
    public string UserName => _userName;
    readonly string _email = context.HttpContext?.User.Email() ?? "no@no.com";
    public string EmailAddress => _email;
}