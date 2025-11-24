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
    public class PostsController : ControllerBase
    {
        private readonly IPostsService postsService;

        public PostsController(IPostsService postsService)
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
        [Route("create")]
        public async Task<ActionResult<PostDto>> CreatePost([FromBody] PostDto postToCreate, CancellationToken cancellationToken = default)
        {
            var post = await postsService.CreatePostAsync(postToCreate, cancellationToken);
            
            return Ok(post);
        }

        [HttpPut]
        [Route("update/{id::guid}")]
        public async Task<ActionResult<PostDto>> UpdatePost(Guid id, [FromBody] UpdatePostRequest request, CancellationToken cancellationToken = default)
        {
            return await postsService.UpdatePostAsync(id, request, cancellationToken);
        }

        [HttpDelete]
        [Route("delete/{id::guid}")]
        public async Task<IActionResult> DeletePost(Guid id, CancellationToken cancellationToken = default)
        {
            await postsService.DeletePostByIdAsync(id, cancellationToken);

            return NoContent();
        }
    }
}