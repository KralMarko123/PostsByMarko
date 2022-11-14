﻿using aspnetserver.Data.Models;
using aspnetserver.Data.Repos.Posts;
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
    public PostsController(IPostsRepository postsRepository, IMapper mapper) : base(mapper)
    {
        this.postsRepository = postsRepository;
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
        bool postCreatedSuccessfully = false;
        if (postToCreate.Title.Length > 0 && postToCreate.Content.Length > 0) postCreatedSuccessfully = await postsRepository.CreatePostAsync(postToCreate);

        if (postCreatedSuccessfully) return Ok("Post was created successfully.");
        else return BadRequest("Error during post creation.");
    }

    [HttpPut]
    [Route("/update-post")]
    [Tags("Posts Endpoint")]
    public async Task<IActionResult> UpdatePostAsync([FromBody] Post postToUpdate)
    {
        bool postUpdatedSuccessfully = await postsRepository.UpdatePostAsync(postToUpdate);

        if (postUpdatedSuccessfully) return Ok("Post was updated successfully.");
        else return BadRequest("Error during post update.");
    }

    [HttpDelete]
    [Route("/delete-post-by-id/{postId}")]
    [Tags("Posts Endpoint")]
    public async Task<IActionResult> DeletePostByIdAsync(int postId)
    {
        bool postDeletedSuccessfully = await postsRepository.DeletePostAsync(postId);

        if (postDeletedSuccessfully) return Ok("Post was deleted successfully.");
        else return BadRequest("Error during post deletion.");
    }
}