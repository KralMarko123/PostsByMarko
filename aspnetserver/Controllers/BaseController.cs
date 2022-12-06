using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    protected readonly IMapper mapper;
    protected string? username;
    protected string? userId;
    protected List<string>? userRoles;

    public BaseController(IMapper mapper)
    {
        this.mapper = mapper;
    }

    protected void LoadUserInfoForRequestBeingExecuted()
    {
        username = HttpContext.User.FindFirstValue(ClaimTypes.Name);
        userId = HttpContext.User.FindFirstValue(ClaimTypes.PrimarySid);
        userRoles = HttpContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
    }
}
