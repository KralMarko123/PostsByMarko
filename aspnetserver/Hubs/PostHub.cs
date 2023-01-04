using aspnetserver.Hubs.Client;
using Microsoft.AspNetCore.SignalR;

namespace aspnetserver.Hubs
{
    public class PostHub : Hub<IPostClient>
    {
        public async Task SendMessageToAll(string message)
        {
            await Clients.All.ReceiveMessage($"Received Message: {message} with Message Id: {Guid.NewGuid()}");
        }

        public async Task SendMessageToOthers(string message)
        {
            await Clients.Others.ReceiveMessage($"Received Message: {message} with Message Id: {Guid.NewGuid()}");
        }
    }
}
