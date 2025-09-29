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
    public PostsController(IPostsService postsService) : base()
    {
        this.postsService = postsService;
    }

    [HttpGet]
    [Route("/getAllPosts")]
    [Tags("Posts Endpoint")]
    public async Task<RequestResult> GetAllPostsAsync()
    {
        LoadRequestClaims();
        return await postsService.GetAllPostsAsync(user);
    }

    [HttpGet]
    [Route("/getPost/{postId}")]
    [Tags("Posts Endpoint")]
    public async Task<RequestResult> GetPostAsync(string postId)
    {
        LoadRequestClaims();
        return await postsService.GetPostByIdAsync(postId, user);
    }

    [HttpPost]
    [Route("/createPost")]
    [Tags("Posts Endpoint")]
    [LimitRequest(MaxRequests = 1, TimeWindow = 3)]
    public async Task<RequestResult> CreatePostAsync([FromBody] Post postToCreate)
    {
        LoadRequestClaims();
        return await postsService.CreatePostAsync(postToCreate, user);
    }

    [HttpPut]
    [Route("/updatePost")]
    [Tags("Posts Endpoint")]
    [LimitRequest(MaxRequests = 1, TimeWindow = 3)]
    public async Task<RequestResult> UpdatePostAsync([FromBody] Post updatedPost)
    {
        LoadRequestClaims();
        return await postsService.UpdatePostAsync(updatedPost, user);
    }

    [HttpDelete]
    [Route("/deletePost/{postId}")]
    [Tags("Posts Endpoint")]
    public async Task<RequestResult> DeletePostAsync(string postId)
    {
        LoadRequestClaims();
        return await postsService.DeletePostByIdAsync(postId, user);
    }

    [HttpPost]
    [Route("/togglePostVisibility/{postId}")]
    [Tags("Posts Endpoint")]
    public async Task<RequestResult> TogglePostVisibilityAsync(string postId)
    {
        LoadRequestClaims();
        return await postsService.TogglePostVisibilityAsync(postId, user);
    }

    [HttpGet]
    [Route("/getPostAuthor/{postId}")]
    [Tags("Posts Endpoint")]
    public async Task<RequestResult> GetPostAuthor(string postId)
    {
        LoadRequestClaims();
        return await postsService.GetPostAuthorDetails(postId);
    }
}
