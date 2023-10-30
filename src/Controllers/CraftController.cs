using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Controllers;

[ApiController]
[Route("[controller]")]
public class CraftController : ControllerBase, ICraftController<CraftSummaryPoco>
{
    private readonly ICraftRepository<CraftSummaryPoco> _craftRepo;
    private readonly ILogger<CraftController> _logger;

    public CraftController(
        ICraftRepository<CraftSummaryPoco> craftRepo,
        ILogger<CraftController> logger
    )
    {
        _craftRepo = craftRepo;
        _logger = logger;
    }

    [HttpGet("{worldId}/{id}")]
    public async Task<CraftSummaryPoco?> GetBestCraft(int worldId, int id)
    {
        _logger.LogInformation($"Fetching craft for item id {id} in world {worldId}");
        return await _craftRepo.GetBestCraftForItem(worldId, id);
    }

    [HttpGet("{worldId}")]
    public async Task<IEnumerable<CraftSummaryPoco>> GetBestCrafts(int worldId)
    {
        _logger.LogInformation($"Fetching best crrafting result for world {worldId}");
        return await _craftRepo.GetBestCraftsForWorld(worldId);
    }
}
