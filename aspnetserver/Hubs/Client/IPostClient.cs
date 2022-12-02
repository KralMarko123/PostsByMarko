namespace aspnetserver.Hubs.Client
{
    public interface IPostClient
    {
        Task ReceiveMessage(string message);
    }
}
