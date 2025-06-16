using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Api.Pocos;
using GilGoblin.Api.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Api.Controllers;

[ApiController]
[Route("api/[controller]/{worldId:int}")]
public class CraftController(ICraftRepository craftRepo, ILogger<CraftController> logger)
    : ControllerBase, ICraftController<CraftSummaryPoco>
{
    [HttpGet]
    public async Task<ActionResult<List<CraftSummaryPoco>>> GetBestAsync(int worldId)
    {
        logger.LogInformation($"Fetching best crafting results for world {worldId}");
        return await craftRepo.GetBestAsync(worldId);
    }
}