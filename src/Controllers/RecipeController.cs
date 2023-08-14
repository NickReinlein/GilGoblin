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

    [HttpGet("{id}")]
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
