using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Api.Crafting;
using GilGoblin.Api.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Api.Controllers;

[ApiController]
[Route("[controller]/{worldId:int}")]
public class CraftController : ControllerBase, ICraftController<CraftSummaryPoco>
{
    private readonly ICraftRepository<CraftSummaryPoco> _craftRepo;
    private readonly ILogger<CraftController> _logger;

    public CraftController(
        ICraftRepository<CraftSummaryPoco> craftRepo,
        ILogger<CraftController> logger
    )
    {
        _craftRepo = craftRepo;
        _logger = logger;
    }

    [HttpGet("")]
    public async Task<IEnumerable<CraftSummaryPoco>> GetBestCrafts(int worldId)
    {
        _logger.LogInformation($"Fetching best crafting result for world {worldId}");
        return await _craftRepo.GetBestCraftsAsync(worldId);
    }

    [HttpGet("{recipeId:int}")]
    public async Task<CraftSummaryPoco?> GetCraft(int worldId, int recipeId)
    {
        _logger.LogInformation($"Fetching craft for world {worldId} with recipeId {recipeId}");
        return await _craftRepo.GetCraftAsync(worldId, recipeId);
    }
}