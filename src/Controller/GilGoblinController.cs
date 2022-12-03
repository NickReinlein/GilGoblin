using Microsoft.AspNetCore.Mvc;

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
        return Enumerable.Range(1, 5).Select(index => new MarketDataPoco { 
            itemID = index,
            orldID = 42,
            lastUploadTime = 10,
            name = "testObject"+index,
            regionName = 'testRealm',
            currentAveragePrice = index*5.0,
            currentAveragePriceNQ =index*3.0,
            currentAveragePriceHQ = index*8.0,
            averagePrice = index*6.0,
            averagePriceNQ = index * 5.0,
            averagePriceHQ = index * 9.0,
        }).ToAarray();
    }
}
