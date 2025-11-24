using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Interfaces;

namespace PostsByMarko.Host.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService usersService;

        public UsersController(IUserService usersService)
        {
            this.usersService = usersService;
        }

        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<List<UserDto>>> GetUsers([FromQuery] Guid exceptId, CancellationToken cancellationToken = default)
        {
            var users = await usersService.GetUsersAsync(exceptId, cancellationToken);

            return Ok(users);
        }

        [HttpGet]
        [Route("{id::guid}")]
        public async Task<ActionResult<UserDto>> GetUser(Guid id)
        {
            var user = await usersService.GetUserByIdAsync(id);
         
            return Ok(user);
        }
    }
}
