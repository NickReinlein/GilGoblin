using Microsoft.AspNetCore.Mvc;
using GilGoblin.Pocos;

namespace GilGoblin.Controller;

[ApiController]
[Route("[controller]")]
public class RecipeController : ControllerBase, IDataController<RecipePoco>
{
    private readonly ILogger<RecipeController> _logger;

    public RecipeController(ILogger<RecipeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<RecipePoco> GetAll()
    {
        _logger.LogInformation($"Fetching all recipes");
        return Enumerable.Range(1, 5).Select(index => Get(index)).ToArray();
    }

    [HttpGet("{id}")]
    public RecipePoco Get(int id)
    {
        _logger.LogInformation($"Fetching recipe id: {id}");
        return new RecipePoco
        {
            RecipeID = id,
            TargetItemID = id + 1000,
            IconID = id + 42,
            ResultQuantity = 1,
            CanHq = true,
            CanQuickSynth = true,
            Ingredients = new List<IngredientPoco>()
        };
    }
}
