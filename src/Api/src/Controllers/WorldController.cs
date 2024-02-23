using System.Collections.Generic;
using System.Linq;
using GilGoblin.Database.Pocos;
using GilGoblin.Api.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WorldController : ControllerBase, IWorldController
{
    private readonly IWorldRepository _worldRepo;
    private readonly ILogger<WorldController> _logger;

    public WorldController(IWorldRepository worldRepo, ILogger<WorldController> logger)
    {
        _worldRepo = worldRepo;
        _logger = logger;
    }

    [HttpGet("{id:int}")]
    public KeyValuePair<int, string> GetWorld(int id)
    {
        _logger.LogInformation($"Fetching world id: {id}");
        return _worldRepo.GetWorld(id);
    }

    [HttpGet]
    public Dictionary<int, string> GetAllWorlds()
    {
        _logger.LogInformation("Fetching all worlds");
        return _worldRepo.GetAllWorlds();
    }
}