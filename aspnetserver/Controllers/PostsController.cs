using aspnetserver.Data.Models;
using aspnetserver.Data.Repos.Posts;
using aspnetserver.Data.Repos.Users;
using AutoMapper;
using Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudentTeacher.Controllers;


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
        Post postToReturn = await postsRepository.GetPostByIdAsync(postId);

        if (postToReturn != null) return Ok(postToReturn);
        else return NotFound($"Post with id: {postId} was not found.");
    }

    [HttpPost]
    [Route("/create-post")]
    [Tags("Posts Endpoint")]
    public async Task<IActionResult> CreatePostAsync([FromBody] Post postToCreate)
    {
        await SetExecutingRequestUser();

        if (postToCreate.Title.Length > 0 && postToCreate.Content.Length > 0)
        {
            postToCreate.Author = user;
            postToCreate.AuthorId = user.Id;
            postToCreate.CreatedDate = DateTime.UtcNow;
            postToCreate.LastUpdatedDate = postToCreate.CreatedDate;

            bool postCreatedSuccessfully = await postsRepository.CreatePostAsync(postToCreate);

            if (postCreatedSuccessfully)
            {
                bool postAddedSuccessfully = await usersRepository.AddPostToUserAsync(user.UserName, postToCreate);

                if (postAddedSuccessfully) return Ok("Post was created successfully.");
                else return BadRequest("Error during post creation.");

            }
            else return BadRequest("Error during post creation.");
        }
        else return BadRequest("Error during post creation.");

    }

    [HttpPut]
    [Route("/update-post")]
    [Tags("Posts Endpoint")]
    public async Task<IActionResult> UpdatePostAsync([FromBody] Post postToUpdate)
    {
        postToUpdate.LastUpdatedDate = DateTime.UtcNow;
        bool postUpdatedSuccessfully = await postsRepository.UpdatePostAsync(postToUpdate);

        if (postUpdatedSuccessfully) return Ok("Post was updated successfully.");
        else return BadRequest("Error during post update.");
    }

    [HttpDelete]
    [Route("/delete-post-by-id/{postId}")]
    [Tags("Posts Endpoint")]
    [Authorize(Roles = "Editor, Admin")]
    public async Task<IActionResult> DeletePostByIdAsync(int postId)
    {
        bool postDeletedSuccessfully = await postsRepository.DeletePostAsync(postId);

        if (postDeletedSuccessfully) return Ok("Post was deleted successfully.");
        else return BadRequest("Error during post deletion.");
    }
}
