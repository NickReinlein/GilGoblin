using Microsoft.AspNetCore.Mvc;
using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Controller;

[ApiController]
[Route("[controller]")]
public class PriceController : ControllerBase, IDataController<MarketDataPoco>
{
    private readonly ILogger<PriceController> _logger;
    private readonly IDataRepository<MarketDataPoco> _priceRepo;

    public PriceController(ILogger<PriceController> logger, IDataRepository<MarketDataPoco> priceRepo)
    {
        _logger = logger;
        _priceRepo = priceRepo;
    }

    [HttpGet]
    public IEnumerable<MarketDataPoco> GetAll()
    {
        _logger.LogInformation($"Fetching all market data");
        var allPrices = _priceRepo.GetAll();
        return allPrices;
    }

    [HttpGet("{id}")]
    public MarketDataPoco Get(int id)
    {
        _logger.LogInformation($"Fetching market data id: {id}");
        var price = _priceRepo.Get(id);
        return price;
    }
}
