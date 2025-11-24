using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Requests;

namespace PostsByMarko.Host.Application.Interfaces
{
    public interface IPostService
    {
        Task<List<PostDto>> GetAllPostsAsync(CancellationToken cancellationToken);
        Task<PostDto> GetPostByIdAsync(Guid Id, CancellationToken cancellationToken);
        Task<PostDto> CreatePostAsync(PostDto post, CancellationToken cancellationToken);
        Task<PostDto> UpdatePostAsync(Guid Id, UpdatePostRequest request, CancellationToken cancellationToken);
        Task DeletePostByIdAsync(Guid postId, CancellationToken cancellationToken);
    }
}
