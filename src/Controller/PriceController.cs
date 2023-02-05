using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.AspNetCore.Mvc;

namespace GilGoblin.Controller;

[ApiController]
[Route("[controller]")]
public class PriceController : ControllerBase, IPriceController
{
    private readonly IPriceRepository _priceRepo;
    private readonly ILogger<PriceController> _logger;

    public PriceController(IPriceRepository priceRepo, ILogger<PriceController> logger)
    {
        _logger = logger;
        _priceRepo = priceRepo;
    }

    [HttpGet("{worldId}")]
    public IEnumerable<PricePoco> GetAll(int worldID)
    {
        _logger.LogInformation($"Fetching all market data for world {worldID}");
        return _priceRepo.GetAll(worldID).Result;
    }

    [HttpGet("{worldId}/{id}")]
    public PricePoco? Get(int worldID, int id)
    {
        _logger.LogInformation($"Fetching market data world {worldID}, id: {id}");
        return _priceRepo.Get(worldID, id).Result;
        ;
    }
}
