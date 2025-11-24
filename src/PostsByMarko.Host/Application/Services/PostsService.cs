using AutoMapper;
using PostsByMarko.Host.Application.Constants;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Data.Entities;
using PostsByMarko.Host.Data.Repositories.Posts;

namespace PostsByMarko.Host.Application.Services
{
    public class PostsService : IPostsService
    {
        private readonly IPostsRepository postsRepository;
        private readonly IUsersService usersService;
        private readonly ICurrentRequestAccessor currentRequestAccessor;
        private readonly IMapper mapper;
        
        public PostsService(IPostsRepository postsRepository, IUsersService usersService,
            ICurrentRequestAccessor currentRequestAccessor, IMapper mapper)
        {
            this.postsRepository = postsRepository;
            this.usersService = usersService;
            this.currentRequestAccessor = currentRequestAccessor;
            this.mapper = mapper;
        }

        public async Task<List<PostDto>> GetAllPostsAsync(CancellationToken cancellationToken = default)
        {
            var currentUser = await usersService.GetUserByIdAsync(Guid.Parse(currentRequestAccessor.Id));
            var userRoles = await usersService.GetRolesForEmailAsync(currentUser.Email);
            var allPosts = await postsRepository.GetPostsAsync(cancellationToken);

            if (!userRoles.Contains(RoleConstants.ADMIN))
            {
                allPosts.RemoveAll(p => p.Hidden && p.AuthorId != currentUser.Id);
            }

            var result = allPosts.Select(p => mapper.Map<PostDto>(p)).ToList();

            return result;
        }

        public async Task<PostDto> GetPostByIdAsync(Guid postId, CancellationToken cancellationToken = default)
        {
            var currentUser = await usersService.GetUserByIdAsync(Guid.Parse(currentRequestAccessor.Id));
            var userRoles = await usersService.GetRolesForEmailAsync(currentUser.Email);
            var post = await postsRepository.GetPostByIdAsync(postId, cancellationToken) ?? throw new KeyNotFoundException($"Post with Id: {postId} was not found");
            
            if (post.Hidden && !userRoles.Contains(RoleConstants.ADMIN) && post.AuthorId != currentUser.Id)
            {
                throw new UnauthorizedAccessException("You are not authorized to view this post");
            } 

            return mapper.Map<PostDto>(post);
        }

        public async Task<PostDto> CreatePostAsync(PostDto postToCreate, CancellationToken cancellationToken = default)
        {
            if(postToCreate.Title.Length == 0 || postToCreate.Content.Length == 0)
            {
                throw new ArgumentException("Post title and content cannot be empty");
            }
           
            postToCreate.CreatedDate = DateTime.UtcNow;
            postToCreate.LastUpdatedDate = postToCreate.CreatedDate;

            var post = mapper.Map<Post>(postToCreate);
            post = await postsRepository.AddPostAsync(post, cancellationToken);

            await postsRepository.SaveChangesAsync(cancellationToken);

            return mapper.Map<PostDto>(post);
        }

        public async Task<PostDto> UpdatePostAsync(Guid postId, UpdatePostRequest request, CancellationToken cancellationToken = default)
        {
            var currentUser = await usersService.GetUserByIdAsync(Guid.Parse(currentRequestAccessor.Id));
            var userRoles = await usersService.GetRolesForEmailAsync(currentUser.Email);
            var post = await postsRepository.GetPostByIdAsync(postId, cancellationToken) ?? throw new KeyNotFoundException($"Post with Id: {postId} was not found");

            if (currentUser.Id != post.AuthorId && !userRoles.Contains(RoleConstants.ADMIN))
            {
                throw new UnauthorizedAccessException("You are not authorized to update this post");
            }

            post.Title = request.Title;
            post.Content = request.Content;
            post.Hidden = request.Hidden;
            post.LastUpdatedDate = DateTime.UtcNow;

            await postsRepository.UpdatePostAsync(post);
            await postsRepository.SaveChangesAsync(cancellationToken);

            var result = mapper.Map<PostDto>(post);

            return result;
        }

        public async Task DeletePostByIdAsync(Guid postId, CancellationToken cancellationToken)
        {
            var currentUser = await usersService.GetUserByIdAsync(Guid.Parse(currentRequestAccessor.Id));
            var userRoles = await usersService.GetRolesForEmailAsync(currentUser.Email);
            var post = await postsRepository.GetPostByIdAsync(postId, cancellationToken) ?? throw new KeyNotFoundException($"Post with Id: {postId} was not found");

            if (currentUser.Id != post.AuthorId && !userRoles.Contains(RoleConstants.ADMIN))
            {
                throw new UnauthorizedAccessException("You are not authorized to delete this post");
            }
            
            await postsRepository.DeletePostAsync(post);
            await postsRepository.SaveChangesAsync(cancellationToken);
        }
    }
}
