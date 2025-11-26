using PostsByMarko.Host.Application.DTOs;

namespace PostsByMarko.Host.Application.Hubs.Client
{
    public interface IPostClient
    {
        Task PostCreated(PostDto post);
        Task PostUpdated(PostDto post);
        Task PostDeleted(Guid id);
    }
}
