using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PostsByMarko.Host.Application.DTOs;
using PostsByMarko.Host.Application.Interfaces;
using PostsByMarko.Host.Application.Responses;
using Microsoft.Extensions.Options;
using PostsByMarko.Host.Application.Configuration;

namespace PostsByMarko.Host.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IUserService usersService;
    private readonly IEmailService emailService;
    private readonly ApplicationUrlConfig applicationUrls;

    public AuthController(IUserService usersService, IEmailService emailService, IOptions<ApplicationUrlConfig> applicationUrls)
    {
        this.usersService = usersService;
        this.emailService = emailService;
        this.applicationUrls = applicationUrls.Value;
    }
    
    [AllowAnonymous]
    [HttpPost]
    [Route("register")]
    public async Task<ActionResult> Register([FromBody] RegistrationDto registrationDto)
    {
        await usersService.CreateUserAsync(registrationDto);

        return Ok("Successfully registered, please check your email and confirm your account before logging in");
    }

    [AllowAnonymous]
    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginDto loginDto, CancellationToken cancellationToken = default)
    {
        var result = await usersService.ValidateUserAsync(loginDto, cancellationToken);

        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("confirm")]
    public async Task<ActionResult> ConfirmEmail([FromQuery] string email, [FromQuery] string token )
    {
        await emailService.ConfirmEmailAsync(email, token);
        
        if (!Uri.TryCreate(applicationUrls.ClientBaseUrl, UriKind.Absolute, out var clientBaseUrl))
        {
            throw new InvalidOperationException("ApplicationUrls:ClientBaseUrl must be an absolute URL.");
        }

        var urlToRedirectTo = new Uri(clientBaseUrl, "/login").ToString();

        return Redirect(urlToRedirectTo);
    }

    [Authorize]
    [HttpGet]
    [Route("validate")]
    public async Task<ActionResult<LoginResponse>> ValidateToken(CancellationToken cancellationToken = default)
    {
        var result = await usersService.ValidateUserWithTokenExistsAsync(cancellationToken);

        return Ok(result);
    }
}
