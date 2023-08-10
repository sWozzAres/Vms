using Microsoft.AspNetCore.SignalR;
using Vms.Web.Server.Hubs;

namespace Vms.Web.Server.Services;

public class ChatHubTest(IHubContext<ChatHub> hubContext, ILogger<ChatHubTest> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogDebug("Broadcasting.");

            await hubContext.Clients.All.SendAsync("ReceiveMessage", "Bob", "A message");

            await Task.Delay(5 * 1000, stoppingToken);
        }
    }
}
