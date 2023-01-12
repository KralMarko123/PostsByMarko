using aspnetserver.Services;
using AutoMapper;
using Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace aspnetserver.Controllers
{
    [Route("")]
    public class EmailController : BaseController
    {
        private readonly IUsersService usersService;
        private readonly IConfiguration configuration;

        public EmailController(IUsersService usersService, IMapper mapper, IConfiguration configuration) : base(mapper)
        {
            this.usersService = usersService;
            this.configuration = configuration;
        }

        [HttpGet]
        [Route("/confirm-email")]
        [Tags("Auth Endpoint")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await usersService.GetUserByUsernameAsync(email);
            var jwtConfig = configuration.GetSection("JwtConfig");
            var urlToRedirectTo = $"{jwtConfig.GetSection("validAudiences").Get<List<string>>().FirstOrDefault()}/login";

            if (user == null) return NotFound($"User with username: {email} was not found");
            if (await usersService.ConfirmEmailForUserAsync(user, token)) return Redirect(urlToRedirectTo);
            else return BadRequest("Error during email confirmation");
        }
    }
}
