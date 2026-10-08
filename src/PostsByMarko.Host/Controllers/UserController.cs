using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Extensions;

namespace PostsByMarko.Host.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService usersService;

        public UserController(IUserService usersService)
        {
            this.usersService = usersService;
        }

        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<List<UserDto>>> GetUsers([FromQuery] PageRequest pagination, [FromQuery] Guid? exceptId = null, CancellationToken cancellationToken = default)
        {
            var users = await usersService.GetUsersAsync(pagination, exceptId, cancellationToken);

            return this.PagedOk(users);
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id, CancellationToken cancellationToken = default)
        {
            var user = await usersService.GetUserByIdAsync(id, cancellationToken);
         
            return Ok(user);
        }
    }
}
