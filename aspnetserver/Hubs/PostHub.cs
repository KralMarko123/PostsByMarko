using aspnetserver.Hubs.Client;
using Microsoft.AspNetCore.SignalR;

namespace aspnetserver.Hubs
{
    public class PostHub : Hub<IPostClient>
    {
        public async Task SendMessageToAll(string message)
        {
            await Clients.All.ReceiveMessage(message);
        }
    }
}
