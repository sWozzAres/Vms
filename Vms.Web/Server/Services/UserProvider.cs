using Vms.Web.Server.Extensions;

namespace Vms.Web.Server.Services;

public class UserProvider(IHttpContextAccessor context) : IUserProvider
{
    public static bool InMigration;

    readonly string _userId = InMigration ? "migrator" : context.HttpContext?.User.UserId()
        ?? throw new InvalidOperationException("UserId is null.");
    public string UserId => _userId;

    readonly string _tenantId = InMigration ? "*" : context.HttpContext?.User.TenantId()
         ?? throw new InvalidOperationException("TenantId is null.");
    public string TenantId => _tenantId;

    readonly string _userName = InMigration ? "Migrator" : context.HttpContext?.User.Name()
        ?? throw new InvalidOperationException("UserName is null.");
    public string UserName => _userName;

    readonly string _email = InMigration ? "migrator@nowhere.com" : context.HttpContext?.User.Email()
        ?? throw new InvalidOperationException("Email is null.");
    public string EmailAddress => _email;
}