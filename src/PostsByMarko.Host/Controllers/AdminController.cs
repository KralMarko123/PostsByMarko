using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Requests;
using PostsByMarko.Host.Application.Responses;

namespace PostsByMarko.Host.Controllers
{
    [ApiController]
    [Route("api/admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IUsersService usersService;

        public AdminController(IUsersService usersService)
        {
            this.usersService = usersService;
        }

        [HttpGet]
        [Route("roles/{email::string}")]
        public async Task<ActionResult<List<string>>> GetRolesForEmail(string email)
        {
            var roles = await usersService.GetRolesForEmailAsync(email);

            return Ok(roles);
        }

        [HttpGet]
        [Route("dashboard")]
        public async Task<ActionResult<List<AdminDashboardResponse>>> GetAdminDashboard(CancellationToken cancellationToken = default)
        {
            var result = await usersService.GetAdminDashboardAsync(cancellationToken);

            return Ok(result);
        }

        [HttpDelete]
        [Route("users/{id::guid}")]
        public async Task<IActionResult> DeleteUser(Guid Id, CancellationToken cancellationToken = default)
        {
            await usersService.DeleteUserAsync(Id, cancellationToken);

            return NoContent();
        }

        [HttpPut]
        [Route("roles")]
        public async Task<ActionResult<List<string>>> UpdateUserRoles(UpdateUserRolesRequest request, CancellationToken cancellationToken = default)
        {
            var result = await usersService.UpdateUserRolesAsync(request, cancellationToken);
            
            return Ok(result);
        }
    }
}
