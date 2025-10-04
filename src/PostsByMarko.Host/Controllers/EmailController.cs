using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsByMarko.Host.Services;

namespace PostsByMarko.Host.Controllers
{
    public class EmailController : BaseController
    {
        private readonly IUsersService usersService;
        private readonly IConfiguration configuration;

        public EmailController(IUsersService usersService, IConfiguration configuration) : base()
        {
            this.usersService = usersService;
            this.configuration = configuration;
        }

        [HttpGet]
        [Route("/confirmEmail")]
        [Tags("Auth Endpoints")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await usersService.GetUserByEmailAsync(email);
            var jwtConfig = configuration.GetSection("JwtConfig");
            var urlToRedirectTo = $"{jwtConfig.GetSection("validAudiences").Get<List<string>>()!.FirstOrDefault()}/login";

            if (user == null) return NotFound($"User with username: {email} was not found");
            if (await usersService.ConfirmEmailForUserAsync(user, token)) return Redirect(urlToRedirectTo);
            else return BadRequest("Error during email confirmation");
        }
    }
}
