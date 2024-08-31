// using System.Collections.Generic;
// using System.Threading.Tasks;
// using GilGoblin.Api.Pocos;
// using GilGoblin.Api.Repository;
// using Microsoft.AspNetCore.Mvc;
// using Microsoft.Extensions.Logging;
//
// namespace GilGoblin.Api.Controllers;
//
// [ApiController]
// [Route("[controller]/{worldId:int}")]
// public class CraftController : ControllerBase, ICraftController<CraftSummaryPoco> 
// {
//     private readonly ICraftRepository _craftRepo;
//     private readonly ILogger<CraftController> _logger;
//
//     public CraftController(ICraftRepository craftRepo, ILogger<CraftController> logger)
//     {
//         _craftRepo = craftRepo;
//         _logger = logger;
//     }
//
//     [HttpGet("")]
//     public async Task<ActionResult<List<CraftSummaryPoco>>> GetBestAsync(int worldId)
//     {
//         _logger.LogInformation($"Fetching best crafting results for world {worldId}");
//         return await _craftRepo.GetBestAsync(worldId);
//     }
// }