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
    private readonly IUserService usersService;
    private readonly IEmailService emailService;
    private readonly IConfiguration configuration;  

    public AuthController(IUserService usersService, IEmailService emailService, IConfiguration configuration)
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
    [Route("confirm")]
    public async Task<ActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token )
    {
        await emailService.ConfirmEmailAsync(email, token);
        
        var jwtConfiguration = configuration.GetSection("JwtConfig");
        var audiences = jwtConfiguration.GetSection("validAudiences").Get<List<string>>();
        var urlToRedirectTo = $"{audiences[0]}/login";

        return Redirect(urlToRedirectTo);
    }
}
