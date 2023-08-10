using Microsoft.AspNetCore.SignalR;

namespace Vms.Web.Server.Services;

/// <summary>
/// Changes SignalR to use the "sub" claim to identify users.
/// </summary>
/// <see cref="https://learn.microsoft.com/en-us/aspnet/core/signalr/authn-and-authz?view=aspnetcore-7.0#use-claims-to-customize-identity-handling"/>
public class SubBasedUserIdProvider : IUserIdProvider
{
    public virtual string GetUserId(HubConnectionContext connection)
    {
        return connection.User?.FindFirst(x => x.Type == "sub")?.Value!;
    }
}
