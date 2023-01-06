using aspnetserver.Data.Models;

namespace aspnetserver.Services
{
    public interface IPostsService
    {
        Task<RequestResult> GetAllPostsAsync(RequestUser user);
        Task<RequestResult> GetPostByIdAsync(int postId, RequestUser user);
        Task<RequestResult> CreatePostAsync(Post postToCreate, RequestUser user);
        Task<RequestResult> UpdatePostAsync(Post updatedPost, RequestUser user);
        Task<RequestResult> DeletePostByIdAsync(int postId, RequestUser user);
        Task<RequestResult> TogglePostVisibilityAsync(int postId, RequestUser user);
        Task<RequestResult> ToggleUserForPostAsync(int postId, string username, RequestUser user);
    }
}
