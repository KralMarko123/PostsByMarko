namespace PostsByMarko.Host.Hubs.Client
{
    public interface IPostClient
    {
        Task ReceiveMessage(string message);
    }
}
