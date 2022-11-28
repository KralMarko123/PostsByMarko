using aspnetserver.Data.Models;
using aspnetserver.Data.Repos.Users;
using aspnetserver.Helper.Exceptions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    private readonly IUsersRepository usersRepository;
    protected readonly IMapper mapper;
    protected User? user;

    public BaseController(IUsersRepository usersRepository, IMapper mapper)
    {
        this.usersRepository = usersRepository;
        this.mapper = mapper;
    }

    protected async Task SetExecutingRequestUser()
    {
        var username = HttpContext.User.FindFirstValue(ClaimTypes.Name);
        user = await usersRepository.GetUserByUsernameAsync(username);

        if(user == null)
        {
            throw new UserNotFoundException($"User with username: {username} was not found.");
        }
    }
}
