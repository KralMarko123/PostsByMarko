using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Decorators;
using PostsByMarko.Host.Services;

namespace PostsByMarko.Host.Controllers
{

    [Route("")]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUsersService usersService;
        public UsersController(IUsersService usersService) : base()
        {
            this.usersService = usersService;
        }

        [HttpGet]
        [Route("/getAllUsers")]
        [Tags("Users Endpoint")]
        [LimitRequest(MaxRequests = 5, TimeWindow = 10)]
        public async Task<RequestResult> GetAllUsersAsync()
        {
            LoadRequestClaims();
            return await usersService.GetAllUsersAsync();
        }

        [HttpGet]
        [Route("/getUser/{id}")]
        [Tags("Users Endpoint")]
        [LimitRequest(MaxRequests = 5, TimeWindow = 10)]
        public async Task<RequestResult> GetUserAsync(string id)
        {
            LoadRequestClaims();
            return await usersService.GetUserByIdAsync(id);
        }

        [HttpGet]
        [Route("/getEmailRoles/{email}")]
        [Tags("Users Endpoint")]
        [LimitRequest(MaxRequests = 5, TimeWindow = 10)]
        [Authorize(Roles = "Admin")]
        public async Task<RequestResult> GetEmailRolesAsync(string email)
        {
            LoadRequestClaims();

            return await usersService.GetRolesForEmailAsync(email);
        }
    }
}
