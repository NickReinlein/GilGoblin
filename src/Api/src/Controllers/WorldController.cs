using System.Collections.Generic;
using GilGoblin.Database.Pocos;
using GilGoblin.Api.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WorldController(IWorldRepository worldRepo, ILogger<WorldController> logger)
    : ControllerBase, IWorldController
{
    [HttpGet("{id:int}")]
    public ActionResult<WorldPoco?> GetWorld(int id)
    {
        if (id <= 0)
            return new BadRequestResult();
        
        logger.LogInformation($"Fetching world id: {id}");
        return worldRepo.Get(id);
    }

    [HttpGet]
    public IEnumerable<WorldPoco> GetAllWorlds()
    {
        logger.LogInformation("Fetching all worlds");
        return worldRepo.GetAll();
    }
}