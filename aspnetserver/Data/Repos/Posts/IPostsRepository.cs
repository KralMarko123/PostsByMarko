using aspnetserver.Data.Models;
namespace aspnetserver.Data.Repos.Posts
{
    public interface IPostsRepository
    {
        Task<List<Post>> GetPostsAsync();

        Task<Post> GetPostByIdAsync(int postId);

        Task<bool> CreatePostAsync(Post postToCreate);

        Task<bool> UpdatePostAsync(Post postToUpdate);

        Task<bool> DeletePostAsync(Post postToDelete);
    }
}
