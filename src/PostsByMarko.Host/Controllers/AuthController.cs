using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Responses;

namespace PostsByMarko.Host.Controllers;

[ApiController]
[Route("api/auth")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IUsersService usersService;
    private readonly IEmailService emailService;
    private readonly IConfiguration configuration;  

    public AuthController(IUsersService usersService, IEmailService emailService, IConfiguration configuration)
    {
        this.usersService = usersService;
        this.emailService = emailService;
        this.configuration = configuration;
    }

    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register([FromBody] RegistrationDto registrationDto)
    {
        await usersService.CreateUserAsync(registrationDto);

        return Ok("Successfully registered, please check your email and confirm your account before logging in");
    }

    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginDto loginDto)
    {
        var result = await usersService.ValidateUserAsync(loginDto);

        return Ok(result);
    }

    [HttpPost]
    [Route("confirmEmail")]
    [AllowAnonymous]
    public async Task<ActionResult> ConfirmEmail([FromBody] ConfirmEmailDto confirmEmailDto)
    {
        await emailService.ConfirmEmailAsync(confirmEmailDto);
        
        var jwtConfiguration = configuration.GetSection("Jwt");
        var urlToRedirectTo = $"{jwtConfiguration.GetSection("validAudiences").Get<List<string>>()!.FirstOrDefault()}/login";

        return Redirect(urlToRedirectTo);
    }
}
