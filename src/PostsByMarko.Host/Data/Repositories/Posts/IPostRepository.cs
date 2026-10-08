using PostsByMarko.Host.Data.Entities;

using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;

namespace PostsByMarko.Host.Data.Repositories.Posts
{
    public interface IPostRepository
    {
        Task<PagedResult<Post>> GetPostsAsync(PageRequest page, Guid viewerId, bool isAdmin, CancellationToken cancellationToken = default);
        Task<Post?> GetPostByIdAsync(Guid Id, CancellationToken cancellationToken = default);
        Task<Post> AddPostAsync(Post postToCreate, CancellationToken cancellationToken = default);
        Task UpdatePostAsync(Post postToUpdate);
        Task DeletePostAsync(Post postToDelete);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
