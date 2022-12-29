using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Controller;

[ApiController]
[Route("[controller]")]
public class PriceController : ControllerBase, IDataController<MarketDataPoco>
{
    private readonly ILogger<PriceController> _logger;
    private readonly IPriceRepository _priceRepo;

    public PriceController(IPriceRepository priceRepo, ILogger<PriceController> logger)
    {
        _logger = logger;
        _priceRepo = priceRepo;
    }

    [HttpGet]
    public IEnumerable<MarketDataPoco> GetAll()
    {
        _logger.LogInformation($"Fetching all market data");
        return _priceRepo.GetAll();
    }

    [HttpGet("{id}")]
    public MarketDataPoco Get(int id)
    {
        _logger.LogInformation($"Fetching market data id: {id}");
        return _priceRepo.Get(id);
        ;
    }
}
