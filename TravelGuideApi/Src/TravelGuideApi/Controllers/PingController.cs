using TravelGuideApi.Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace TravelGuideApi.Controllers;

[ApiController]
// Keep at least one root route which return status OK
[Route("")]
[Route("api/[controller]")]
public class PingController(IOptions<Config> configOptions) : ControllerBase
{
    private readonly Config _config = configOptions.Value;

    /// <summary>
    /// Just a ping endpoint. Gets the version of artifact
    /// </summary>
    /// <returns>Ping value from config</returns>
    [HttpGet]
    public string Ping()
    {
        return _config.PingValue;
    }
}
