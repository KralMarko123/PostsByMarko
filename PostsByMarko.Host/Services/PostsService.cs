using PostsByMarko.Host.Builders;
using PostsByMarko.Host.Constants;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Responses;
using PostsByMarko.Host.Data.Repos.Posts;
using PostsByMarko.Host.Data.Repos.Users;
using Serilog;

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

            if (!user.UserRoles.Contains("Admin"))
            {
                allPosts.RemoveAll(p => p.IsHidden && !p.AllowedUsers.Contains(user.Username) && p.UserId != user.UserId);
            }

            return new RequestResultBuilder().Ok().WithPayload(allPosts).Build();
        }

        public async Task<RequestResult> GetPostByIdAsync(string postId, RequestUser user)
        {
            var post = await postsRepository.GetPostByIdAsync(postId);

            if (post == null) return new RequestResultBuilder().NotFound().WithMessage($"Post with Id: {postId} was not found").Build();

            if (post.IsHidden == true && !post.AllowedUsers.Contains(user.Username) && !user.UserRoles.Contains("Admin"))
                return new RequestResultBuilder().Unauthorized().WithMessage($"Post with Id: {postId} is hidden").Build();

            var postAuthor = await usersRepository.GetUserByIdAsync(post.UserId);

            return new RequestResultBuilder().Ok().WithPayload(new PostDetailsResponse
            {
                Post = post,
                AuthorFirstName = postAuthor.FirstName,
                AuthorLastName = postAuthor.LastName,
            }).Build();
        }

        public async Task<RequestResult> CreatePostAsync(Post postToCreate, RequestUser user)
        {
            var badRequest = new RequestResultBuilder().BadRequest().WithMessage("Error during post creation").Build();

            if (postToCreate.Title.Length > 0 && postToCreate.Content.Length > 0)
            {
                postToCreate.UserId = user.UserId;
                postToCreate.CreatedDate = DateTime.UtcNow;
                postToCreate.LastUpdatedDate = postToCreate.CreatedDate;

                var newlyCreatedPost = await postsRepository.CreatePostAsync(postToCreate);
                var postAddedToUser = await usersRepository.AddPostToUserAsync(user.Username, newlyCreatedPost);

                if (postAddedToUser.Succeeded)
                {
                    Log.Logger.Information($"Successfully created Post with Id: {newlyCreatedPost.PostId}");
                    return new RequestResultBuilder().Created().WithMessage("Post was created successfully").WithPayload(newlyCreatedPost).Build();
                }
                else return badRequest;
            }
            else return badRequest;
        }

        public async Task<RequestResult> UpdatePostAsync(Post updatedPost, RequestUser user)
        {
            var badRequest = new RequestResultBuilder().BadRequest().WithMessage("Error during post update").Build();
            var postToUpdate = await postsRepository.GetPostByIdAsync(updatedPost.PostId.ToString());

            if (postToUpdate != null && (user.UserId == postToUpdate.UserId || user.UserRoles.Any(x => AppConstants.appRoles.Any(y => y.Name == x))))
            {
                postToUpdate.Title = updatedPost.Title;
                postToUpdate.Content = updatedPost.Content;

                var postUpdatedSuccessfully = await postsRepository.UpdatePostAsync(postToUpdate);

                if (postUpdatedSuccessfully)
                {
                    Log.Logger.Information($"Successfully updated Post with Id: {postToUpdate.PostId}");
                    return new RequestResultBuilder().Ok().WithMessage("Post was updated successfully").Build();
                }
                else return badRequest;
            }
            else return badRequest;
        }

        public async Task<RequestResult> DeletePostByIdAsync(string postId, RequestUser user)
        {
            var badRequest = new RequestResultBuilder().BadRequest().WithMessage("Error during post deletion").Build();
            var postToDelete = await postsRepository.GetPostByIdAsync(postId);

            if (postToDelete != null && (user.UserId == postToDelete.UserId || user.UserRoles.Contains("Admin")))
            {
                var postDeletedSuccessfully = await postsRepository.DeletePostAsync(postToDelete);

                if (postDeletedSuccessfully)
                {
                    Log.Logger.Information($"Successfully deleted Post with Id: {postToDelete.PostId}");
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

            if (post.UserId == user.UserId || user.UserRoles.Contains("Admin"))
            {
                post.IsHidden = !post.IsHidden;

                var postToggledSuccessfully = await postsRepository.UpdatePostAsync(post);

                if (postToggledSuccessfully) return new RequestResultBuilder().Ok().WithMessage("Post visibility was toggled successfully").Build();
                else return badRequest;
            }
            else return unauthorizedRequest;
        }

        public async Task<RequestResult> ToggleUserForPostAsync(string postId, string username, RequestUser requestUser)
        {
            var badRequest = new RequestResultBuilder().BadRequest().WithMessage($"Error while toggling user for post").Build();
            var unauthorizedRequest = new RequestResultBuilder().Unauthorized().WithMessage($"Unauthorized to toggle user for post").Build();
            var post = await postsRepository.GetPostByIdAsync(postId);

            if (post.UserId == requestUser.UserId || requestUser.UserRoles.Contains("Admin"))
            {
                var user = await usersRepository.GetUserByUsernameAsync(username);

                if (user != null)
                {
                    if (!post.AllowedUsers.Contains(user.UserName)) post.AllowedUsers.Add(user.UserName);
                    else post.AllowedUsers.Remove(user.UserName);

                    var postUpdatedSuccessfully = await postsRepository.UpdatePostAsync(post);

                    if (postUpdatedSuccessfully) return new RequestResultBuilder().Ok().WithMessage($"User was toggled successfully").Build();
                    else return badRequest;
                }
                else return badRequest;
            }
            else return unauthorizedRequest;
        }
    }
}
