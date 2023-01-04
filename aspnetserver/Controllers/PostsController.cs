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

            bool postCreatedSuccessfully = await postsRepository.CreatePostAsync(postToCreate);

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

            bool postUpdatedSuccessfully = await postsRepository.UpdatePostAsync(postToUpdate);

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
            bool postDeletedSuccessfully = await postsRepository.DeletePostAsync(postToDelete);

            if (postDeletedSuccessfully) return Ok("Post was deleted successfully");
            else return BadRequest("Error during post deletion");
        }
        else return BadRequest("Error during post deletion");
    }

    [HttpPost]
    [Route("/toggle-post-visibility/{postId}")]
    [Tags("Posts Endpoint")]
    [LimitRequest(MaxRequests = 1, TimeWindow = 10)]
    public async Task<IActionResult> TogglePostVisibilityAsync(int postId)
    {
        LoadUserInfoForRequestBeingExecuted();

        var post = await postsRepository.GetPostByIdAsync(postId);

        if (post.UserId == userId || userRoles.Contains("Admin"))
        {
            post.IsHidden = !post.IsHidden;

            bool postToggledSuccessfully = await postsRepository.UpdatePostAsync(post);

            if (postToggledSuccessfully) return Ok("Post visibility was toggled successfully");
            else return BadRequest("Error during post visibility toggle");
        }
        else return Unauthorized("Unauthorized to toggle Post's visibility");
    }
}
