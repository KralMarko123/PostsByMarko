using PostsByMarko.Host.Data.Models;

namespace PostsByMarko.Host.Services
{
    public interface IPostsService
    {
        Task<RequestResult> GetAllPostsAsync(RequestUser user);
        Task<RequestResult> GetPostByIdAsync(string postId, RequestUser user);
        Task<RequestResult> CreatePostAsync(Post postToCreate, RequestUser user);
        Task<RequestResult> UpdatePostAsync(Post updatedPost, RequestUser user);
        Task<RequestResult> DeletePostByIdAsync(string postId, RequestUser user);
        Task<RequestResult> TogglePostVisibilityAsync(string postId, RequestUser user);
    }
}
