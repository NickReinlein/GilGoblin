using Microsoft.AspNetCore.Mvc;
using GilGoblin.Pocos;

namespace GilGoblin.Controller;

[ApiController]
[Route("[controller]")]
public class PriceController : ControllerBase
{
    private readonly ILogger<PriceController> _logger;

    public PriceController(ILogger<PriceController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<MarketDataPoco> Get()
    {
        return Enumerable
            .Range(1, 5)
            .Select(
                index =>
                    new MarketDataPoco
                    {
                        ItemID = index,
                        WorldID = 42,
                        LastUploadTime = 10,
                        Name = "testObject" + index,
                        RegionName = "MountFuji",
                        AverageListingPrice = index * 5f,
                        AverageListingPriceNQ = index * 3f,
                        AverageListingPriceHQ = index * 8f,
                        AverageSold = index * 6f,
                        AverageSoldNQ = index * 5f,
                        AverageSoldHQ = index * 9f,
                    }
            )
            .ToArray();
    }

    [HttpGet("{id}")]
    public MarketDataPoco Get(int id)
    {
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
