using aspnetserver.Data.Models;
namespace aspnetserver.Data.Repos.Posts
{
    public interface IPostsRepository
    {
        Task<List<Post>> GetPostsAsync();

        Task<Post> GetPostByIdAsync(string postId);

        Task<Post> CreatePostAsync(Post postToCreate);

        Task<bool> UpdatePostAsync(Post postToUpdate);

        Task<bool> DeletePostAsync(Post postToDelete);
    }
}
