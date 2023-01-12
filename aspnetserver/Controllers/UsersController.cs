using aspnetserver.Data.Models;
using aspnetserver.Decorators;
using aspnetserver.Services;
using AutoMapper;
using Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aspnetserver.Controllers
{

    [Route("")]
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUsersService usersService;
        public UsersController(IUsersService usersService, ILogger logger, IMapper mapper) : base(logger, mapper)
        {
            this.usersService = usersService;
        }

        [HttpGet]
        [Route("/get-all-users")]
        [Tags("Users Endpoint")]
        [LimitRequest(MaxRequests = 5, TimeWindow = 10)]
        public async Task<RequestResult> GetAllUsernamesAsync()
        {
            LoadUserInfoForRequestBeingExecuted();
            return await usersService.GetAllUsernamesAsync();
        }
    }
}
