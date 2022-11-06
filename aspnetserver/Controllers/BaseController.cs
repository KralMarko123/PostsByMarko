using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace StudentTeacher.Controllers;

public class BaseApiController : ControllerBase
{
    protected readonly IMapper _mapper;

    public BaseApiController(IMapper mapper)
    {
        _mapper = mapper;
    }
}
