using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Crafting;

public class RecipeGrocer : IRecipeGrocer
{
    private readonly IRecipeRepository _recipes;
    private readonly ILogger<RecipeGrocer> _log;

    public RecipeGrocer(IRecipeRepository recipes, ILogger<RecipeGrocer> log)
    {
        _recipes = recipes;
        _log = log;
    }

    public async Task<IEnumerable<IngredientPoco?>> BreakdownRecipe(int recipeID)
    {
        _log.LogInformation("Fetching recipe ID {RecipeID} from gateway", recipeID);
        var recipe = await _recipes.Get(recipeID);
        if (recipe is null)
        {
            _log.LogInformation("No recipe was found with ID {RecipeID} ", recipeID);
            return Array.Empty<IngredientPoco>();
        }

        var ingredientList = await BreakDownIngredientEntirely(recipe.Ingredients);

        return ingredientList;
    }

    public async Task<IEnumerable<IngredientPoco?>> BreakDownIngredientEntirely(
        IEnumerable<IngredientPoco?> ingredientList
    )
    {
        var ingredientsBrokenDownList = new List<IngredientPoco>();
        _log.LogInformation(
            "Breaking down {IngCount} ingredients in ingredient list",
            ingredientList.Count()
        );
        foreach (var ingredient in ingredientList)
        {
            if (ingredient is null)
                continue;

            var itemID = ingredient.ItemID;
            _log.LogDebug("Breaking down item ID {ItemID}", itemID);
            var breakdownIngredient = await BreakdownItem(itemID);
            if (breakdownIngredient.Any(i => i is not null))
            {
                _log.LogDebug("Found {IngCount} ingredients", breakdownIngredient.Count());
                ingredientsBrokenDownList.AddRange(
                    breakdownIngredient.Where(i => i is not null).ToList<IngredientPoco>()
                );
            }
            else
            {
                _log.LogDebug("Did not find any items to break down item ID {ItemID}", itemID);
                ingredientsBrokenDownList.Add(ingredient);
            }
        }
        _log.LogInformation(
            "Breakdown complete. {IngCount} ingredients returned",
            ingredientList.Count()
        );
        return ingredientsBrokenDownList;
    }

    public async Task<IEnumerable<IngredientPoco?>> BreakdownItem(int itemID)
    {
        _log.LogInformation("Fetching recipes for item ID {ItemID} from gateway", itemID);
        var ingredientRecipes = await _recipes.GetRecipesForItem(itemID);
        _log.LogInformation("No recipe was found for item ID {ItemID} ", itemID);

        foreach (var ingredientRecipe in ingredientRecipes)
        {
            if (ingredientRecipe is null)
                continue;

            var ingredientRecipeID = ingredientRecipe.ID;
            if (CanMakeRecipe(ingredientRecipeID))
            {
                var recipeIngredients = await BreakdownRecipe(ingredientRecipeID);
                foreach (var ingredient in recipeIngredients)
                {
                    if (ingredient is null)
                        continue;

                    ingredient.Quantity *= ingredientRecipe.ResultQuantity;
                }
                return recipeIngredients;
            }
        }
        return Array.Empty<IngredientPoco>();
    }

    public static bool CanMakeRecipe(int recipeID)
    {
        //add functionality here to check for crafting levels per recipe
        return recipeID > 0;
    }
}
