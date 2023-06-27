using AutoMapper;
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
        public UsersController(IUsersService usersService, IMapper mapper) : base(mapper)
        {
            this.usersService = usersService;
        }

        [HttpGet]
        [Route("/get-all-users")]
        [Tags("Users Endpoint")]
        [LimitRequest(MaxRequests = 5, TimeWindow = 10)]
        public async Task<RequestResult> GetAllUsernamesAsync()
        {
            LoadRequestClaims();
            return await usersService.GetAllUsernamesAsync();
        }
    }
}
