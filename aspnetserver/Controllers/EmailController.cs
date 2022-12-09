using aspnetserver.Data.Models;
using AutoMapper;
using Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static aspnetserver.Constants.AppConstants;

namespace aspnetserver.Controllers
{
    [Route("")]
    public class EmailController : BaseController
    {
        private readonly UserManager<User> userManager;
        private readonly IConfiguration configuration;


        public EmailController(IMapper mapper, UserManager<User> userManager, IConfiguration configuration) : base(mapper)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        [HttpGet]
        [Route("/confirm-email")]
        [Tags("Auth Endpoint")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            var user = await userManager.FindByNameAsync(email);

            if (user == null) return new NotFoundObjectResult(user);

            var result = await userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {

                var environment = configuration["Environment"];
                var redirectToLogin = "";

                switch (environment)
                {
                    case "DEV":
                        redirectToLogin = $"{devClientUrl}/login";
                        break;
                    case "PRD":
                        redirectToLogin = $"{prdClientUrl}/login";
                        break;
                    default:
                        break;
                }
                return Redirect(redirectToLogin);
            }
            else return new BadRequestObjectResult(result);
        }
    }
}
