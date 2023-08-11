using Microsoft.AspNetCore.SignalR;
using Vms.Application.Services;
using Vms.Web.Server.Hubs;

namespace Vms.Web.Server.Services;

public class NotifyFollowersViaSignalR(
    IHubContext<ChatHub> hubContext,
    ILogger<NotifyFollowersViaSignalR> logger) : INotifyFollowers
{
    public async Task NotifyAsync(IEnumerable<string> userIds)
    {
        logger.LogDebug("Notifying followers via signalR {@followers}.", userIds);

        await hubContext.Clients.Users(userIds).SendAsync("ReceiveNotification");
    }
}
