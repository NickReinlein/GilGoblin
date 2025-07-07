using System.Collections.Generic;
using GilGoblin.Database.Pocos;
using GilGoblin.Api.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Api.Controllers;

[ApiController]
[Route("api/[controller]/{worldId:int}")]
public class PriceController(IPriceRepository<PricePoco> priceRepo, ILogger<PriceController> logger)
    : ControllerBase, IPriceController
{
    [HttpGet("{id:int}/{isHq:bool}")]
    public PricePoco? Get(int worldId, int id, bool isHq)
    {
        logger.LogInformation(
            "Fetching market data for item id {Id} in world {WorldId}, hq: {IsHq}",
            id,
            worldId,
            isHq);
        return priceRepo.Get(worldId, id, isHq);
    }

    [HttpGet]
    public IEnumerable<PricePoco> GetAll(int worldId)
    {
        logger.LogInformation("Fetching all market data for world {WorldId}", worldId);
        return priceRepo.GetAll(worldId);
    }
}