using Microsoft.AspNetCore.Mvc;
using GilGoblin.Pocos;

namespace GilGoblin.Controller;

[ApiController]
[Route("[controller]")]
public class CraftController : ControllerBase, ICraftController<CraftSummaryPoco>
{
    private readonly ILogger<CraftController> _logger;

    public CraftController(ILogger<CraftController> logger)
    {
        _logger = logger;
    }

    [HttpGet("{worldId}/{id}")]
    public CraftSummaryPoco GetCraft(int worldId, int id)
    {
        _logger.LogInformation($"Fetching craft for item id {id} in world {worldId}");
        return new CraftSummaryPoco
        {
            WorldID = worldId,
            ItemID = id,
            Name = "testItem" + id,
            AverageListingPrice = 300,
            AverageSold = 280,
            CraftingCost = 180,
            CraftingProfitVsListings = 300 - 180,
            CraftingProfitVsSold = 280 - 180,
            VendorPrice = 50
        };
    }

    [HttpGet("{world}")]
    public IEnumerable<CraftSummaryPoco> GetBestCrafts(int worldId)
    {
        _logger.LogInformation($"Fetching best crafts for world {worldId}");
        return Enumerable.Range(1, 5).Select(index => GetCraft(worldId, index)).ToArray();
    }
}
