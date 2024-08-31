// using System.Collections.Generic;
// using GilGoblin.Database.Pocos;
// using GilGoblin.Api.Repository;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
//
// namespace GilGoblin.Api.Controllers;
//
// [ApiController]
// [Route("[controller]/{worldId:int}")]
// public class PriceController : ControllerBase, IPriceController
// {
//     private readonly IPriceRepository<PricePoco> _priceRepo;
//     private readonly ILogger<PriceController> _logger;
//
//     public PriceController(IPriceRepository<PricePoco> priceRepo, ILogger<PriceController> logger)
//     {
//         _logger = logger;
//         _priceRepo = priceRepo;
//     }
//
//     [HttpGet("{id:int}")]
//     public PricePoco? Get(int worldId, int id)
//     {
//         _logger.LogInformation($"Fetching market data world {worldId}, id: {id}");
//         return _priceRepo.Get(worldId, id);
//     }
//
//     [HttpGet("")]
//     public IEnumerable<PricePoco> GetAll(int worldId)
//     {
//         _logger.LogInformation($"Fetching all market data for world {worldId}");
//         return _priceRepo.GetAll(worldId);
//     }
// }
