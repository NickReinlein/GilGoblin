using System.Collections.Generic;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Controllers;

[ApiController]
[Route("[controller]")]
public class RecipeController : ControllerBase, IDataController<RecipePoco>
{
    private readonly IRecipeRepository _recipeRepo;
    private readonly ILogger<RecipeController> _logger;

    public RecipeController(IRecipeRepository recipeRepo, ILogger<RecipeController> logger)
    {
        _recipeRepo = recipeRepo;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IEnumerable<RecipePoco>> GetAll()
    {
        _logger.LogInformation($"Fetching all recipes");
        return await _recipeRepo.GetAll();
    }

    [HttpGet("{id}")]
    public async Task<RecipePoco?> Get(int id)
    {
        _logger.LogInformation($"Fetching recipe id: {id}");
        return await _recipeRepo.Get(id);
    }
}
