using aspnetserver.Data.Models.Dtos;
using aspnetserver.Data.Repos.Users;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace StudentTeacher.Controllers;

[Route("/auth")]
[ApiController]
public class AuthController : BaseApiController
{
    private readonly IUsersRepository usersRepository;
    public AuthController(IUsersRepository usersRepository, IMapper mapper) : base(mapper)
    {
        this.usersRepository = usersRepository;
    }

    [HttpPost]
    [Route("/register")]
    public async Task<IActionResult> RegisterUser([FromBody] UserRegistrationDto userRegistration)
    {

        var userResult = await usersRepository.RegisterUserAsync(userRegistration);
        return !userResult.Succeeded ? new BadRequestObjectResult(userResult) : StatusCode(201);
    }

    //[HttpPost]
    //[Route("/login")]
    //public async Task<IActionResult> Authenticate([FromBody] UserLoginDto user)
    //{
    //    return !await _repository.UserAuthentication.ValidateUserAsync(user)
    //    ? Unauthorized()
    //        : Ok(new { Token = await _repository.UserAuthentication.CreateTokenAsync() });
    //}

}
