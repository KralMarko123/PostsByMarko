using aspnetserver.Constants;
using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Responses;
using aspnetserver.Data.Repos.Posts;
using aspnetserver.Data.Repos.Users;
using aspnetserver.Decorators;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers;

[Route("")]
[Authorize]
public class PostsController : BaseController
{
    private readonly IPostsRepository postsRepository;
    private readonly IUsersRepository usersRepository;
    public PostsController(IPostsRepository postsRepository, IUsersRepository usersRepository, IMapper mapper) : base(mapper)
    {
        this.postsRepository = postsRepository;
        this.usersRepository = usersRepository;
    }

    [HttpGet]
    [Route("/get-all-posts")]
    [Tags("Posts Endpoint")]
    [LimitRequest(MaxRequests = 5, TimeWindow = 10)]
    public async Task<List<Post>> GetPostsAsync()
    {
        LoadUserInfoForRequestBeingExecuted();

        var allPosts = await postsRepository.GetPostsAsync();
        if (userRoles.Contains("Admin")) return allPosts;

        allPosts.RemoveAll(p => p.IsHidden == true && !p.AllowedUsers.Contains(username));
        return allPosts;
    }

    [HttpGet]
    [Route("/get-post-by-id/{postId}")]
    [Tags("Posts Endpoint")]
    [LimitRequest(MaxRequests = 2, TimeWindow = 5)]
    public async Task<IActionResult> GetPostByIdAsync(int postId)
    {
        LoadUserInfoForRequestBeingExecuted();

        var post = await postsRepository.GetPostByIdAsync(postId);

        if (post == null) return NotFound($"Post with Id: {postId} was not found");
        if (post.IsHidden == true && !post.AllowedUsers.Contains(username) && !userRoles.Contains("Admin")) return Unauthorized($"Post with Id: {postId} is hidden");

        var postAuthor = await usersRepository.GetUserByIdAsync(post.UserId);
        return Ok(new PostDetailsResponse()
        {
            Post = post,
            AuthorFirstName = postAuthor.FirstName,
            AuthorLastName = postAuthor.LastName,
        });
    }

    [HttpPost]
    [Route("/create-post")]
    [Tags("Posts Endpoint")]
    [LimitRequest(MaxRequests = 1, TimeWindow = 10)]
    public async Task<IActionResult> CreatePostAsync([FromBody] Post postToCreate)
    {
        LoadUserInfoForRequestBeingExecuted();

        if (postToCreate.Title.Length > 0 && postToCreate.Content.Length > 0)
        {
            postToCreate.UserId = userId;
            postToCreate.AllowedUsers.Add(username);

            var postCreatedSuccessfully = await postsRepository.CreatePostAsync(postToCreate);

            if (postCreatedSuccessfully)
            {
                var postAddedToUser = await usersRepository.AddPostToUserAsync(username, postToCreate);

                if (postAddedToUser.Succeeded) return Ok("Post was created successfully");
                else return BadRequest("Error during post creation");
            }
            else return BadRequest("Error during post creation");
        }
        else return BadRequest("Error during post creation");

    }

    [HttpPut]
    [Route("/update-post")]
    [Tags("Posts Endpoint")]
    [LimitRequest(MaxRequests = 1, TimeWindow = 10)]
    public async Task<IActionResult> UpdatePostAsync([FromBody] Post updatedPost)
    {
        LoadUserInfoForRequestBeingExecuted();

        var postToUpdate = await postsRepository.GetPostByIdAsync(updatedPost.PostId);

        if (postToUpdate != null && (userId == postToUpdate.UserId || userRoles.Any(x => AppConstants.appRoles.Any(y => y.Name == x))))
        {
            postToUpdate.Title = updatedPost.Title;
            postToUpdate.Content = updatedPost.Content;

            var postUpdatedSuccessfully = await postsRepository.UpdatePostAsync(postToUpdate);

            if (postUpdatedSuccessfully) return Ok("Post was updated successfully");
            else return BadRequest("Error during post update");
        }
        else return BadRequest("Error during post update");
    }

    [HttpDelete]
    [Route("/delete-post-by-id/{postId}")]
    [Tags("Posts Endpoint")]
    [LimitRequest(MaxRequests = 1, TimeWindow = 5)]
    public async Task<IActionResult> DeletePostByIdAsync(int postId)
    {
        LoadUserInfoForRequestBeingExecuted();

        var postToDelete = await postsRepository.GetPostByIdAsync(postId);

        if (postToDelete != null && (userId == postToDelete.UserId || userRoles.Contains("Admin")))
        {
            var postDeletedSuccessfully = await postsRepository.DeletePostAsync(postToDelete);

            if (postDeletedSuccessfully) return Ok("Post was deleted successfully");
            else return BadRequest("Error during post deletion");
        }
        else return BadRequest("Error during post deletion");
    }

    [HttpPost]
    [Route("/toggle-post-visibility/{postId}")]
    [Tags("Posts Endpoint")]
    [LimitRequest(MaxRequests = 1, TimeWindow = 3)]
    public async Task<IActionResult> TogglePostVisibilityAsync(int postId)
    {
        LoadUserInfoForRequestBeingExecuted();

        var post = await postsRepository.GetPostByIdAsync(postId);

        if (post.UserId == userId || userRoles.Contains("Admin"))
        {
            post.IsHidden = !post.IsHidden;

            var postToggledSuccessfully = await postsRepository.UpdatePostAsync(post);

            if (postToggledSuccessfully) return Ok("Post visibility was toggled successfully");
            else return BadRequest("Error during post visibility toggle");
        }
        else return Unauthorized("Unauthorized to toggle Post's visibility");
    }

    [HttpPost]
    [Route("/add-user-to-post/{postId}")]
    [Tags("Posts Endpoint")]
    [LimitRequest(MaxRequests = 1, TimeWindow = 3)]
    public async Task<IActionResult> AllowUserForPost(int postId, [FromBody] string username)
    {
        LoadUserInfoForRequestBeingExecuted();

        var post = await postsRepository.GetPostByIdAsync(postId);
        if (post.UserId == userId || userRoles.Contains("Admin"))
        {
            var userToAdd = await usersRepository.GetUserByUsernameAsync(username);

            if (userToAdd != null && !post.AllowedUsers.Contains(userToAdd.UserName))
            {
                post.AllowedUsers.Add(userToAdd.UserName);
                var postUpdatedSuccessfully = await postsRepository.UpdatePostAsync(post);

                if (postUpdatedSuccessfully) return Ok("Post visibility was toggled successfully");
                else return BadRequest("Error while adding user to post");
            }
            else return BadRequest("Invalid user to add");
        }
        else return Unauthorized("Unauthorized to add user to this Post");
    }
}
