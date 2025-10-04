using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsByMarko.Host.Data.Models;
using PostsByMarko.Host.Data.Models.Dtos;
using PostsByMarko.Host.Helper;
using PostsByMarko.Host.Services;
using System.Net;

namespace PostsByMarko.Host.Controllers;

[AllowAnonymous]
public class AuthController : BaseController
{
    private readonly IUsersService usersService;
    private readonly IEmailHelper emailHelper;

    public AuthController(IUsersService usersService, IEmailHelper emailHelper) : base()
    {
        this.usersService = usersService;
        this.emailHelper = emailHelper;
    }

    [HttpPost]
    [Route("/register")]
    [Tags("Auth Endpoints")]
    public async Task<RequestResult> RegisterUser([FromBody] UserRegistrationDto userRegistration)
    {
        var result = await usersService.MapAndCreateUserAsync(userRegistration);

        if (result.StatusCode.Equals(HttpStatusCode.Created)) await SendEmailConfirmationLinkToUser(userRegistration.Email!);

        return result;
    }

    [HttpPost]
    [Route("/login")]
    [Tags("Auth Endpoint")]
    public async Task<RequestResult> AuthenticateUser([FromBody] UserLoginDto userLogin)
    {
        var result = await usersService.ValidateUserAsync(userLogin);

        if (result.StatusCode.Equals(HttpStatusCode.Forbidden)) await SendEmailConfirmationLinkToUser(userLogin.Email!);

        return result;
    }

    private async Task SendEmailConfirmationLinkToUser(string username)
    {
        var user = await usersService.GetUserByEmailAsync(username);
        var token = await usersService.GenerateEmailConfirmationTokenForUserAsync(user);
        var confirmationLink = Url.Action("ConfirmEmail", "Email", new { token, email = user.Email }, Request.Scheme);
        var subject = $"Please confirm the registration for {user.Email}";
        var body = $"Your account has been successfully created. Please click on the following link to confirm your registration: {confirmationLink}";

        await emailHelper.SendEmail(user.FirstName!, user.LastName!, user.Email, subject, body);
    }
}
