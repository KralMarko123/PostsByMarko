using aspnetserver.Data.Models;
using aspnetserver.Data.Models.Dtos;
using aspnetserver.Data.Models.Responses;
using aspnetserver.Data.Repos.Users;
using aspnetserver.Helper;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Controllers;

[Route("")]
[AllowAnonymous]
public class AuthController : BaseController
{
    private readonly IUsersRepository usersRepository;
    private readonly UserManager<User> userManager;
    private readonly IJwtHelper jwtHelper;

    public AuthController(IUsersRepository usersRepository, UserManager<User> userManager, IJwtHelper jwtHelper, IMapper mapper) : base(mapper)
    {
        this.usersRepository = usersRepository;
        this.userManager = userManager;
        this.jwtHelper = jwtHelper;
    }

    [HttpPost]
    [Route("/register")]
    [Tags("Auth Endpoint")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto userRegistration)
    {

        var result = await usersRepository.MapAndCreateUserAsync(userRegistration);

        if (result.IsValid)
        {
            var user = await usersRepository.GetUserByUsernameAsync(userRegistration.UserName);
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = Url.Action("ConfirmEmail", "Email", new { token, email = user.Email }, Request.Scheme);
            var emailHelper = new EmailHelper();
            await emailHelper.SendEmail(user.FirstName, user.LastName, user.Email, confirmationLink);

            return StatusCode(201);
        }
        else return new BadRequestObjectResult(result.Reason);

    }

    [HttpPost]
    [Route("/login")]
    [Tags("Auth Endpoint")]
    public async Task<IActionResult> Authenticate([FromBody] UserLoginDto userLogin)
    {
        var result = await usersRepository.ValidateUserAsync(userLogin);

        if (!result.IsValid)
        {
            if (result.Reason == "NOT CONFIRMED")
            {
                var user = await usersRepository.GetUserByUsernameAsync(userLogin.UserName);
                var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.Action("ConfirmEmail", "Email", new { token, email = user.Email }, Request.Scheme);
                var emailHelper = new EmailHelper();
                await emailHelper.SendEmail(user.FirstName, user.LastName, user.Email, confirmationLink);
            }
            return new UnauthorizedObjectResult(result.Reason);
        }
        else
        {
            var loggedInUser = await usersRepository.GetUserByUsernameAsync(userLogin.UserName);

            return Ok(new LoginResponse()
            {
                Token = await jwtHelper.CreateTokenAsync(loggedInUser),
                UserId = loggedInUser.Id,
                FirstName = loggedInUser.FirstName,
                LastName = loggedInUser.LastName,
                Roles = await usersRepository.GetUserRolesByUsernameAsync(loggedInUser.UserName),
            });
        }
    }
}
