using Microsoft.AspNetCore.Mvc;
using GilGoblin.Pocos;

namespace GilGoblin.Controller;

[ApiController]
[Route("[prices]")]
public class GilGoblinPriceController : ControllerBase
{
    private readonly ILogger<GilGoblinPriceController> _logger;

    public GilGoblinPriceController(ILogger<GilGoblinPriceController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "GetPrice")]
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
                        Name = "testObject",
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
}
