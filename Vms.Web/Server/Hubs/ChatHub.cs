﻿using Microsoft.AspNetCore.SignalR;

namespace Vms.Web.Server.Hubs;

[Authorize]
public class ChatHub : Hub
{
    public async Task SendMessage(string user, string message)
    {
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}