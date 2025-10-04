namespace PostsByMarko.Host.Hubs.Client
{
    public interface IMessageClient
    {
        Task ReceiveMessage(string message);
    }
}
