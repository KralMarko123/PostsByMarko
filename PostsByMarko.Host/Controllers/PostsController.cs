using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Decorators;
using PostsByMarko.Host.Services;

namespace PostsByMarko.Host.Controllers;

[Route("")]
[Authorize]
public class PostsController : BaseController
{
    private readonly IPostsService postsService;
    public PostsController(IPostsService postsService, IMapper mapper) : base(mapper)
    {
        this.postsService = postsService;
    }

    [HttpGet]
    [Route("/get-all-posts")]
    [Tags("Posts Endpoint")]
    public async Task<RequestResult> GetPostsAsync()
    {
        LoadRequestClaims();
        return await postsService.GetAllPostsAsync(user);
    }

    [HttpGet]
    [Route("/get-post-by-id/{postId}")]
    [Tags("Posts Endpoint")]
    public async Task<RequestResult> GetPostByIdAsync(string postId)
    {
        LoadRequestClaims();
        return await postsService.GetPostByIdAsync(postId, user);
    }

    [HttpPost]
    [Route("/create-post")]
    [Tags("Posts Endpoint")]
    [LimitRequest(MaxRequests = 1, TimeWindow = 3)]
    public async Task<RequestResult> CreatePostAsync([FromBody] Post postToCreate)
    {
        LoadRequestClaims();
        return await postsService.CreatePostAsync(postToCreate, user);
    }

    [HttpPut]
    [Route("/update-post")]
    [Tags("Posts Endpoint")]
    [LimitRequest(MaxRequests = 1, TimeWindow = 3)]
    public async Task<RequestResult> UpdatePostAsync([FromBody] Post updatedPost)
    {
        LoadRequestClaims();
        return await postsService.UpdatePostAsync(updatedPost, user);
    }

    [HttpDelete]
    [Route("/delete-post-by-id/{postId}")]
    [Tags("Posts Endpoint")]
    public async Task<RequestResult> DeletePostByIdAsync(string postId)
    {
        LoadRequestClaims();
        return await postsService.DeletePostByIdAsync(postId, user);
    }

    [HttpPost]
    [Route("/toggle-post-visibility/{postId}")]
    [Tags("Posts Endpoint")]
    public async Task<RequestResult> TogglePostVisibilityAsync(string postId)
    {
        LoadRequestClaims();
        return await postsService.TogglePostVisibilityAsync(postId, user);
    }

    [HttpPost]
    [Route("/toggle-user-for-post/{postId}")]
    [Tags("Posts Endpoint")]
    public async Task<RequestResult> ToggleUserForPost(string postId, [FromBody] string username)
    {
        LoadRequestClaims();
        return await postsService.ToggleUserForPostAsync(postId, username, user);
    }
}
