using Microsoft.AspNetCore.Mvc;
using GilGoblin.Pocos;

namespace GilGoblin.Controller;

[ApiController]
[Route("[controller]")]
public class CraftController : ControllerBase, ICraftController<CraftResultPoco>
{
    private readonly ILogger<CraftController> _logger;

    public CraftController(ILogger<CraftController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{worldId}/{id}")]
    public CraftResultPoco Get(int worldId, int id)
    {
        _logger.LogInformation($"Fetching craft for item id {id} in world {worldId}");
        return new CraftResultPoco();
    }
}
