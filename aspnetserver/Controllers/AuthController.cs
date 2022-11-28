using aspnetserver.Data.Models.Dtos;
using aspnetserver.Data.Models.Responses;
using aspnetserver.Data.Repos.Users;
using aspnetserver.Helper;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Controllers;

[Route("")]
public class AuthController : BaseController
{
    private readonly IUsersRepository usersRepository;
    private readonly IJwtHelper jwtHelper;

    public AuthController(IUsersRepository usersRepository, IJwtHelper jwtHelper, IMapper mapper) : base(usersRepository, mapper)
    {
        this.usersRepository = usersRepository;
        this.jwtHelper = jwtHelper;
    }

    [HttpPost]
    [Route("/register")]
    [AllowAnonymous]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto userRegistration)
    {

        var userResult = await usersRepository.RegisterUserAsync(userRegistration);
        return !userResult.Succeeded ? new BadRequestObjectResult(userResult) : StatusCode(201);
    }

    [HttpPost]
    [Route("/login")]
    [AllowAnonymous]
    public async Task<IActionResult> Authenticate([FromBody] UserLoginDto userLogin)
    {
        if (!await usersRepository.ValidateUserAsync(userLogin)) return Unauthorized();
        else
        {
            return Ok(new LoginResponse()
            {
                Token = await jwtHelper.CreateTokenAsync(),
                Profile = await usersRepository.GetUserProfileByUsername(userLogin.UserName)
            });
        }
    }
}
