using AutoMapper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace StudentTeacher.Controllers;

[ApiController]
[EnableCors("postsCorsPolicy")]
public class BaseApiController : ControllerBase
{
    protected readonly IMapper _mapper;
    public BaseApiController(IMapper mapper)
    {
        _mapper = mapper;
    }
}
