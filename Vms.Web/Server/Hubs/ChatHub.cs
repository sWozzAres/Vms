﻿using Microsoft.AspNetCore.SignalR;

namespace Vms.Web.Server.Hubs;

public class ChatHub : Hub
{
    
    public async Task SendMessage(string user, string message)
    {
        Clients.
        await Clients.All.SendAsync("ReceiveMessage", user, message);
    }
}