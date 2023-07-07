using Microsoft.AspNetCore.Mvc;
using PostsByMarko.Host.Data.Models;
using System.Security.Claims;

namespace PostsByMarko.Host.Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    protected RequestUser user;
    public BaseController()
    {
        user = new RequestUser { };
    }

    protected void LoadRequestClaims()
    {
        user = new RequestUser
        {
            UserId = HttpContext.User.FindFirstValue(ClaimTypes.PrimarySid),
            Email = HttpContext.User.FindFirstValue(ClaimTypes.Email),
            Roles = HttpContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        };
    }
}
