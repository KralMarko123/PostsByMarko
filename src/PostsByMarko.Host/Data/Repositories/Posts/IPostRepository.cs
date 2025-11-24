using PostsByMarko.Host.Data.Entities;

namespace PostsByMarko.Host.Data.Repositories.Posts
{
    public interface IPostRepository
    {
        Task<List<Post>> GetPostsAsync(CancellationToken cancellationToken);
        Task<Post?> GetPostByIdAsync(Guid Id, CancellationToken cancellationToken);
        Task<Post> AddPostAsync(Post postToCreate, CancellationToken cancellationToken);
        Task UpdatePostAsync(Post postToUpdate);
        Task DeletePostAsync(Post postToDelete);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
