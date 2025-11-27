using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Requests;

namespace PostsByMarko.Host.Controllers
{
    [ApiController]
    [Route("api/post")]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPostService postsService;

        public PostController(IPostService postsService)
        {
            this.postsService = postsService;
        }

        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<List<PostDto>>> GetPosts(CancellationToken cancellationToken = default)
        {
            var result = await postsService.GetAllPostsAsync(cancellationToken);

            return Ok(result);
        }

        [HttpGet]
        [Route("{id::guid}")]
        public async Task<ActionResult<PostDto>> GetPost(Guid id, CancellationToken cancellationToken = default)
        {
            var post = await postsService.GetPostByIdAsync(id, cancellationToken);

            return Ok(post);        
        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<PostDto>> CreatePost([FromBody] CreatePostRequest createRequest, CancellationToken cancellationToken = default)
        {
            var post = await postsService.CreatePostAsync(createRequest, cancellationToken);
            
            return Ok(post);
        }

        [HttpPut]
        [Route("{id::guid}")]
        public async Task<ActionResult<PostDto>> UpdatePost(Guid id, [FromBody] UpdatePostRequest request, CancellationToken cancellationToken = default)
        {
            var updatedPost = await postsService.UpdatePostAsync(id, request, cancellationToken);
            
            return Ok(updatedPost);
        }

        [HttpDelete]
        [Route("{id::guid}")]
        public async Task<IActionResult> DeletePost(Guid id, CancellationToken cancellationToken = default)
        {
            await postsService.DeletePostByIdAsync(id, cancellationToken);

            return NoContent();
        }
    }
}