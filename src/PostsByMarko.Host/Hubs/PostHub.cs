﻿using Microsoft.AspNetCore.SignalR;
using PostsByMarko.Host.Hubs.Client;

namespace PostsByMarko.Host.Hubs
{
    public class PostHub : Hub<IPostClient>
    {
        public async Task SendMessageToAll(string message)
        {
            await Clients.All.ReceiveMessage($"Received Message: '{message}' at {DateTime.UtcNow}");
        }

        public async Task SendMessageToOthers(string message)
        {
            await Clients.Others.ReceiveMessage($"Received Message: '{message}' at {DateTime.UtcNow}");
        }
    }
}
