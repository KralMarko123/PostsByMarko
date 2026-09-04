using PostsByMarko.Host.Application.DTOs;

namespace PostsByMarko.Host.Application.Hubs.Client
{
    public interface IPostClient
    {
        Task PostCreated(PostChangeDto notification);
        Task PostUpdated(PostChangeDto notification);
        Task PostDeleted(Guid id);
    }
}
