using Microsoft.AspNetCore.Mvc;
using GilGoblin.Pocos;

namespace GilGoblin.Controller;

[ApiController]
[Route("[controller]")]
public class RecipeController : ControllerBase
{
    private readonly ILogger<RecipeController> _logger;

    public RecipeController(ILogger<RecipeController> logger)
    {
        _logger = logger;
    }

    [HttpGet]
    public IEnumerable<RecipePoco> Get()
    {
        return Enumerable.Range(1, 5).Select(index => Get(index)).ToArray();
    }

    [HttpGet("{id}")]
    public RecipePoco Get(int id)
    {
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
