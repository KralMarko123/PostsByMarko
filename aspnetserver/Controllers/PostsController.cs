using aspnetserver.Constants;
using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Responses;
using aspnetserver.Data.Repos.Posts;
using aspnetserver.Data.Repos.Users;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace Controllers;

[Route("")]
[Authorize]
public class PostsController : BaseController
{
    private readonly IPostsRepository postsRepository;
    private readonly IUsersRepository usersRepository;
    public PostsController(IPostsRepository postsRepository, IUsersRepository usersRepository, IMapper mapper) : base(usersRepository, mapper)
    {
        this.postsRepository = postsRepository;
        this.usersRepository = usersRepository;
    }

    [HttpGet]
    [Route("/get-all-posts")]
    [Tags("Posts Endpoint")]
    public async Task<List<Post>> GetPostsAsync()
    {
        return await postsRepository.GetPostsAsync();
    }

    [HttpGet]
    [Route("/get-post-by-id/{postId}")]
    [Tags("Posts Endpoint")]
    public async Task<IActionResult> GetPostByIdAsync(int postId)
    {
        var post = await postsRepository.GetPostByIdAsync(postId);
        var postAuthor = await usersRepository.GetUserByIdAsync(post.UserId);


        if (post != null)
        {
            return Ok(new PostResponse()
            {
                Post = post,
                Author = await usersRepository.GetUserProfileByUsername(postAuthor.UserName)
            });
        }
        else return NotFound($"Post with id: {postId} was not found.");
    }

    [HttpGet]
    [Route("/get-my-posts")]
    [Tags("Posts Endpoint")]
    public async Task<List<Post>> GetPostsForUser()
    {
        await SetExecutingRequestUser();

        var userPosts = await postsRepository.GetPostsAsync();
        userPosts = userPosts.Where(p => p.UserId == user.Id).ToList();

        return userPosts;
    }

    [HttpPost]
    [Route("/create-post")]
    [Tags("Posts Endpoint")]
    public async Task<IActionResult> CreatePostAsync([FromBody] Post postToCreate)
    {
        await SetExecutingRequestUser();

        if (postToCreate.Title.Length > 0 && postToCreate.Content.Length > 0)
        {
            postToCreate.UserId = user.Id;

            bool postCreatedSuccessfully = await postsRepository.CreatePostAsync(postToCreate);

            if (postCreatedSuccessfully)
            {
                var postAddedToUser = await usersRepository.AddPostToUserAsync(user.UserName, postToCreate);

                if (postAddedToUser.Succeeded) return Ok("Post was created successfully.");
                else return BadRequest("Error during post creation.");
            }
            else return BadRequest("Error during post creation.");
        }
        else return BadRequest("Error during post creation.");

    }

    [HttpPut]
    [Route("/update-post")]
    [Tags("Posts Endpoint")]
    public async Task<IActionResult> UpdatePostAsync([FromBody] Post updatedPost)
    {
        await SetExecutingRequestUser();

        var postToUpdate = await postsRepository.GetPostByIdAsync(updatedPost.PostId);
        var userRoles = await usersRepository.GetUserRolesByUsername(user.UserName);

        if (postToUpdate != null && (user.Id == postToUpdate.UserId || userRoles.Any(x => AppConstants.appRoles.Any(y => y.Name == x))))
        {
            postToUpdate.Title = updatedPost.Title;
            postToUpdate.Content = updatedPost.Content;

            bool postUpdatedSuccessfully = await postsRepository.UpdatePostAsync(postToUpdate);

            if (postUpdatedSuccessfully) return Ok("Post was updated successfully.");
            else return BadRequest("Error during post update.");
        }
        else return BadRequest("Error during post update.");
    }

    [HttpDelete]
    [Route("/delete-post-by-id/{postId}")]
    [Tags("Posts Endpoint")]
    [Authorize(Roles = "Editor, Admin")]
    public async Task<IActionResult> DeletePostByIdAsync(int postId)
    {
        await SetExecutingRequestUser();

        var postToDelete = await postsRepository.GetPostByIdAsync(postId);
        var userRoles = await usersRepository.GetUserRolesByUsername(user.UserName);

        if (postToDelete != null && (user.Id == postToDelete.UserId || userRoles.Contains("Admin")))
        {
            bool postDeletedSuccessfully = await postsRepository.DeletePostAsync(postToDelete);

            if (postDeletedSuccessfully) return Ok("Post was deleted successfully.");
            else return BadRequest("Error during post deletion.");
        }
        else return BadRequest("Error during post deletion.");
    }
}
