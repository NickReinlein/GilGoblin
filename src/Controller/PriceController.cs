using Microsoft.AspNetCore.Mvc;
using GilGoblin.Pocos;

namespace GilGoblin.Controller;

[ApiController]
[Route("[controller]")]
public class PriceController : ControllerBase, IDataController<MarketDataPoco>
{
    private readonly ILogger<PriceController> _logger;

    public PriceController(ILogger<PriceController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<MarketDataPoco> Get()
    {
        _logger.LogInformation($"Fetching all market data");
        return Enumerable.Range(1, 5).Select(index => Get(index)).ToArray();
    }

    [HttpGet("{id}")]
    public MarketDataPoco Get(int id)
    {
        _logger.LogInformation($"Fetching market data id: {id}");
        return new MarketDataPoco
        {
            ItemID = id,
            WorldID = 42,
            LastUploadTime = 10,
            Name = "testObject" + id,
            RegionName = "MountFuji",
            AverageListingPrice = id * 5f,
            AverageListingPriceNQ = id * 3f,
            AverageListingPriceHQ = id * 8f,
            AverageSold = id * 6f,
            AverageSoldNQ = id * 5f,
            AverageSoldHQ = id * 9f,
        };
    }
}
