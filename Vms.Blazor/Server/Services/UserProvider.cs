using Vms.Domain.Services;

namespace Vms.Blazor.Server.Services;

public class UserProvider : IUserProvider
{
    readonly IHttpContextAccessor Context;

    public UserProvider(IHttpContextAccessor context)
        => Context = context;

    public Guid UserId => Guid.Empty;
    public Guid TenantId => Guid.Empty;
}
