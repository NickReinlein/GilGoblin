using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Controller;

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

    [HttpGet("{worldId}")]
    public IEnumerable<CraftSummaryPoco> GetBestCrafts(int worldId)
    {
        _logger.LogInformation($"Fetching best crafts for world {worldId}");
        return Enumerable.Range(1, 5).Select(index => GetCraft(worldId, index)).ToArray();
    }
}
