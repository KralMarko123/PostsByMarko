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
    protected readonly ILogger logger;
    public BaseController(ILogger logger, IMapper mapper)
    {
        this.logger = logger;
        this.mapper = mapper;
    }

    protected void LoadUserInfoForRequestBeingExecuted()
    {
        logger.LogInformation("Loading user information from incoming request");

        user = new RequestUser
        {
            UserId = HttpContext.User.FindFirstValue(ClaimTypes.PrimarySid),
            Username = HttpContext.User.FindFirstValue(ClaimTypes.Name),
            UserRoles = HttpContext.User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList()
        };
    }
}
