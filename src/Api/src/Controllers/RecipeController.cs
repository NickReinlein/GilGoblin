using System.Collections.Generic;
using GilGoblin.Database.Pocos;
using GilGoblin.Api.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Api.Controllers;

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

    [HttpGet("{id:int}")]
    public RecipePoco? Get(int id)
    {
        _logger.LogInformation($"Fetching recipe id: {id}");
        return _recipeRepo.Get(id);
    }

    [HttpGet]
    public IEnumerable<RecipePoco> GetAll()
    {
        _logger.LogInformation($"Fetching all recipes");
        return _recipeRepo.GetAll();
    }
}