using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PostsByMarko.Host.Data.Models;
using System.Security.Claims;

namespace PostsByMarko.Host.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    protected readonly IMapper mapper;
    protected RequestUser user;
    public BaseController(IMapper mapper)
    {
        this.mapper = mapper;
        user = new RequestUser { };
    }

    protected void LoadRequestClaims()
    {
        user = new RequestUser
        {
            UserId = HttpContext.User.FindFirstValue(ClaimTypes.PrimarySid),
            Username = HttpContext.User.FindFirstValue(ClaimTypes.Name),
            UserRoles = HttpContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        };
    }
}
