namespace PostsByMarko.Host.Application.Hubs.Client
{
    public interface IMessageClient
    {
        Task ReceiveMessage(string message);
    }
}
