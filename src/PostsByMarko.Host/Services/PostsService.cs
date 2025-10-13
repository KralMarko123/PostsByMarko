using PostsByMarko.Host.Builders;
using PostsByMarko.Host.Constants;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Responses;
using PostsByMarko.Host.Data.Repos.Posts;
using PostsByMarko.Host.Data.Repos.Users;

namespace PostsByMarko.Host.Services
{
    public class PostsService : IPostsService
    {
        private readonly IPostsRepository postsRepository;
        private readonly IUsersRepository usersRepository;

        public PostsService(IPostsRepository postsRepository, IUsersRepository usersRepository)
        {
            this.postsRepository = postsRepository;
            this.usersRepository = usersRepository;
        }

        public async Task<RequestResult> GetAllPostsAsync(RequestUser user)
        {
            var allPosts = await postsRepository.GetPostsAsync();

            if (!user.Roles!.Contains(RoleConstants.ADMIN))
            {
                allPosts.RemoveAll(p => p.IsHidden && p.AuthorId != user.Id);
            }

            return new RequestResultBuilder().Ok().WithPayload(allPosts).Build();
        }

        public async Task<RequestResult> GetPostByIdAsync(string postId, RequestUser user)
        {
            var notfoundRequest = new RequestResultBuilder().NotFound().WithMessage($"Post with Id: {postId} was not found").Build();
            var unauthorizedRequest = new RequestResultBuilder().Unauthorized().WithMessage($"Post with Id: {postId} is hidden").Build();
            var post = await postsRepository.GetPostByIdAsync(postId);

            if (post == null) 
                return notfoundRequest;
            
            if (post.IsHidden && (!user.Roles!.Contains(RoleConstants.ADMIN) && post.AuthorId != user.Id)) 
                return unauthorizedRequest;

            return new RequestResultBuilder()
                .Ok()
                .WithPayload(post)
                .Build();
        }

        public async Task<RequestResult> CreatePostAsync(Post postToCreate, RequestUser user)
        {
            var badRequest = new RequestResultBuilder().BadRequest().WithMessage("Error during post creation").Build();

            if (postToCreate.Title.Length > 0 && postToCreate.Content.Length > 0)
            {
                postToCreate.AuthorId = user.Id;
                postToCreate.CreatedDate = DateTime.UtcNow;
                postToCreate.LastUpdatedDate = postToCreate.CreatedDate;

                if (postToCreate.Id == string.Empty)
                {
                    postToCreate.Id = Guid.NewGuid().ToString();
                }

                try
                {
                    var actualUser = await usersRepository.GetUserByIdAsync(user.Id);
                    var newlyCreatedPost = await postsRepository.CreatePostAsync(postToCreate);
                    var postSuccessfullyAddedToUser = await usersRepository.AddPostToUserAsync(actualUser, newlyCreatedPost);

                    return new RequestResultBuilder()
                        .Created()
                        .WithMessage("Post was created successfully")
                        .WithPayload(newlyCreatedPost)
                        .Build();
                }
                catch (Exception)
                {
                    return badRequest;
                }

            }
            else return badRequest;
        }

        public async Task<RequestResult> UpdatePostAsync(Post updatedPost, RequestUser user)
        {
            var badRequest = new RequestResultBuilder().BadRequest().WithMessage("Error during post update").Build();
            var postToUpdate = await postsRepository.GetPostByIdAsync(updatedPost.Id.ToString());

            if (postToUpdate != null && (user.Id == postToUpdate.AuthorId || user.Roles!.Contains(RoleConstants.ADMIN)))
            {
                postToUpdate.Title = updatedPost.Title;
                postToUpdate.Content = updatedPost.Content;
                postToUpdate.LastUpdatedDate = DateTime.UtcNow;

                var postUpdatedSuccessfully = await postsRepository.UpdatePostAsync(postToUpdate);

                if (postUpdatedSuccessfully) 
                    return new RequestResultBuilder()
                        .Ok()
                        .WithMessage("Post was updated successfully")
                        .Build();
                else return badRequest;
            }
            else return badRequest;
        }

        public async Task<RequestResult> DeletePostByIdAsync(string postId, RequestUser user)
        {
            var badRequest = new RequestResultBuilder().BadRequest().WithMessage("Error during post deletion").Build();
            var postToDelete = await postsRepository.GetPostByIdAsync(postId);

            if (postToDelete != null && (user.Id == postToDelete.AuthorId || user.Roles!.Contains(RoleConstants.ADMIN)))
            {
                var postDeletedSuccessfully = await postsRepository.DeletePostAsync(postToDelete);

                if (postDeletedSuccessfully)
                {
                    await usersRepository.RemovePostFromUserAsync(user.Email, postToDelete);
                    return new RequestResultBuilder().Ok().WithMessage("Post was deleted successfully").Build();
                }
                else return badRequest;
            }
            else return badRequest;
        }

        public async Task<RequestResult> TogglePostVisibilityAsync(string postId, RequestUser user)
        {
            var badRequest = new RequestResultBuilder().BadRequest().WithMessage("Error during post visibility toggle").Build();
            var unauthorizedRequest = new RequestResultBuilder().Unauthorized().WithMessage("Unauthorized to toggle Post's visibility").Build();
            var post = await postsRepository.GetPostByIdAsync(postId);

            if (post.AuthorId == user.Id || user.Roles!.Contains(RoleConstants.ADMIN))
            {
                post.IsHidden = !post.IsHidden;

                var postToggledSuccessfully = await postsRepository.UpdatePostAsync(post);

                if (postToggledSuccessfully) return new RequestResultBuilder().Ok().WithMessage("Post visibility was toggled successfully").Build();
                else return badRequest;
            }
            else return unauthorizedRequest;
        }

        public async Task<RequestResult> GetPostAuthorDetails(string postId)
        {
            var postNotfoundRequest = new RequestResultBuilder().NotFound().WithMessage($"Post with Id: {postId} was not found").Build();
            var post = await postsRepository.GetPostByIdAsync(postId);

            if (post == null)
                return postNotfoundRequest;

            var authorNotFoundRequest = new RequestResultBuilder().NotFound().WithMessage($"Author with Id: {post.AuthorId} was not found").Build();
            var author = await usersRepository.GetUserByIdAsync(post.AuthorId);

            if (author == null)
                return authorNotFoundRequest;

            var authorResponse = new AuthorDetailsResponse(author);

            return new RequestResultBuilder()
                .Ok()
                .WithPayload(authorResponse)
                .Build();
        }
    }
}
