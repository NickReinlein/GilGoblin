// using System.Collections.Generic;
// using GilGoblin.Pocos;
// using GilGoblin.Repository;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;

// namespace GilGoblin.Controllers;

// [ApiController]
// [Route("[controller]")]
// public class CraftController : ControllerBase, ICraftController<CraftSummaryPoco>
// {
//     private readonly ICraftRepository<CraftSummaryPoco> _craftRepo;
//     private readonly ILogger<CraftController> _logger;

//     public CraftController(
//         ICraftRepository<CraftSummaryPoco> craftRepo,
//         ILogger<CraftController> logger
//     )
//     {
//         _craftRepo = craftRepo;
//         _logger = logger;
//     }

//     [HttpGet("{worldId}/{id}")]
//     public CraftSummaryPoco? GetCraft(int worldId, int id)
//     {
//         _logger.LogInformation($"Fetching craft for item id {id} in world {worldId}");
//         return _craftRepo.GetCraft(worldId, id).Result;
//     }

//     [HttpGet("{worldId}")]
//     public IEnumerable<CraftSummaryPoco?> GetBestCrafts(int worldId)
//     {
//         _logger.LogInformation($"Fetching best crrafting result for world {worldId}");
//         return _craftRepo.GetBestCrafts(worldId).Result;
//     }
// }
