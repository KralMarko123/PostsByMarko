namespace PostsByMarko.Host.Application.Hubs.Client
{
    public interface IPostClient
    {
        Task ReceiveMessage(string message);
    }
}
