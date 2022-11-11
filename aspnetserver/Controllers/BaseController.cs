using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Controllers;

[ApiController]
public class BaseController : ControllerBase
{
    protected readonly IMapper _mapper;
    public BaseController(IMapper mapper)
    {
        _mapper = mapper;
    }
}
