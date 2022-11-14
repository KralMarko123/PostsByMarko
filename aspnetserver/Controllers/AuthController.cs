﻿using aspnetserver.Data.Models.Dtos;
using aspnetserver.Data.Repos.Users;
using AutoMapper;
using Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudentTeacher.Controllers;

[Route("")]
public class AuthController : BaseController
{
    private readonly IUsersRepository usersRepository;
    public AuthController(IUsersRepository usersRepository, IMapper mapper) : base(mapper)
    {
        this.usersRepository = usersRepository;
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
    public async Task<IActionResult> Authenticate([FromBody] UserLoginDto user)
    {
        return !await usersRepository.ValidateUserAsync(user)
            ? Unauthorized()
            : Ok(new { Token = await usersRepository.CreateTokenAsync(), UserDetails = await usersRepository.GetUserDetailsForUsernameAsync(user.UserName) });
    }

}