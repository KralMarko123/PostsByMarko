using aspnetserver.Data.Repos.Users;
using aspnetserver.Decorators;
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
        private readonly IUsersRepository usersRepository;
        public UsersController(IUsersRepository usersRepository, IMapper mapper) : base(mapper)
        {
            this.usersRepository = usersRepository;
        }

        [HttpGet]
        [Route("/get-all-users")]
        [Tags("Users Endpoint")]
        [LimitRequest(MaxRequests = 5, TimeWindow = 10)]
        public async Task<List<string>> GetAllUsernamesAsync()
        {
            LoadUserInfoForRequestBeingExecuted();

            var allUsernames = await usersRepository.GetAllUsernamesAsync();
            allUsernames.Remove(username);

            return allUsernames;
        }
    }
}
