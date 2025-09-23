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
        [Route("/getUserRoles")]
        [Tags("Users Endpoint")]
        [LimitRequest(MaxRequests = 5, TimeWindow = 10)]
        public async Task<List<string>> GetUserRolesAsync()
        {
            LoadRequestClaims();

            var userToSearch = await usersService.GetUserByIdAsync(user.UserId);

            return await usersService.GetRolesForUserAsync(userToSearch!);
        }
    }
}
