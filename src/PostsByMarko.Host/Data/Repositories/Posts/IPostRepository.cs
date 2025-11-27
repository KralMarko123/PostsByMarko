using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Data.Repositories.Posts
{
    public interface IPostRepository
    {
        Task<List<Post>> GetPostsAsync(CancellationToken cancellationToken = default);
        Task<Post?> GetPostByIdAsync(Guid Id, CancellationToken cancellationToken = default);
        Task<Post> AddPostAsync(Post postToCreate, CancellationToken cancellationToken = default);
        Task UpdatePostAsync(Post postToUpdate);
        Task DeletePostAsync(Post postToDelete);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
