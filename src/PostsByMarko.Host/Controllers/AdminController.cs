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
        private readonly IAdminService adminService;

        public AdminController(IAdminService adminService)
        {
            this.adminService = adminService;
        }

        [HttpGet]
        [Route("roles")]
        public async Task<ActionResult<List<string>>> GetRolesForEmail([FromQuery] string email, CancellationToken cancellationToken = default)
        {
            var roles = await adminService.GetRolesForEmailAsync(email, cancellationToken);

            return Ok(roles);
        }

        [HttpGet]
        [Route("dashboard")]
        public async Task<ActionResult<List<AdminDashboardResponse>>> GetAdminDashboard(CancellationToken cancellationToken = default)
        {
            var result = await adminService.GetAdminDashboardAsync(cancellationToken);

            return Ok(result);
        }

        [HttpDelete]
        [Route("users/{id::guid}")]
        public async Task<IActionResult> DeleteUser(Guid Id, CancellationToken cancellationToken = default)
        {
            await adminService.DeleteUserByIdAsync(Id, cancellationToken);

            return NoContent();
        }

        [HttpPut]
        [Route("roles")]
        public async Task<ActionResult<List<string>>> UpdateUserRoles([FromBody] UpdateUserRolesRequest request, CancellationToken cancellationToken = default)
        {
            var result = await adminService.UpdateUserRolesAsync(request, cancellationToken);
            
            return Ok(result);
        }
    }
}
