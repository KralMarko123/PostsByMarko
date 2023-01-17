using aspnetserver.Data.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    protected readonly IMapper mapper;
    protected RequestUser? user;
    public BaseController(IMapper mapper)
    {
        this.mapper = mapper;
    }

    protected void LoadUserInfoForRequestBeingExecuted()
    {
        user = new RequestUser
        {
            UserId = HttpContext.User.FindFirstValue(ClaimTypes.PrimarySid),
            Username = HttpContext.User.FindFirstValue(ClaimTypes.Name),
            UserRoles = HttpContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        };
    }
}
