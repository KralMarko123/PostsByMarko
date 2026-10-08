using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;

namespace PostsByMarko.Host.Application.Interfaces
{
    public interface IPostService
    {
        Task<PagedResult<PostDto>> GetAllPostsAsync(PageRequest page, CancellationToken cancellationToken = default);
        Task<PostDto> GetPostByIdAsync(Guid Id, CancellationToken cancellationToken = default);
        Task<PostDto> CreatePostAsync(CreatePostRequest request, CancellationToken cancellationToken = default);
        Task<PostDto> UpdatePostAsync(Guid Id, UpdatePostRequest request, CancellationToken cancellationToken = default);
        Task DeletePostByIdAsync(Guid Id, CancellationToken cancellationToken = default);
    }
}
