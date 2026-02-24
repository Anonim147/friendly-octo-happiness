using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace TravelGuideApi.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
public abstract class BaseApiController(IMediator mediator) : ControllerBase
{
    protected IMediator Mediator => mediator;
}
