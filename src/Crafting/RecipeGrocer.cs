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

    public IEnumerable<IngredientPoco> BreakdownRecipe(int recipeID)
    {
        _log.LogInformation("Fetching recipe ID {RecipeID} from gateway", recipeID);
        var recipe = _recipes.Get(recipeID);
        if (recipe is null)
        {
            _log.LogInformation("No recipe was found with ID {RecipeID} ", recipeID);
            return Array.Empty<IngredientPoco>();
        }

        var ingredientList = BreakDownIngredientEntirely(recipe.Ingredients);

        return ingredientList;
    }

    public List<IngredientPoco> BreakDownIngredientEntirely(List<IngredientPoco> ingredientList)
    {
        var ingredientsBrokenDownList = new List<IngredientPoco>();
        _log.LogInformation(
            "Breaking down {IngCount} ingredients in ingredient list",
            ingredientList.Count
        );
        foreach (var ingredient in ingredientList)
        {
            var itemID = ingredient.ItemID;
            _log.LogDebug("Breaking down item ID {ItemID}", itemID);
            var breakdownIngredient = BreakdownItem(itemID);
            if (breakdownIngredient.Any())
            {
                _log.LogDebug("Found {IngCount} ingredients", breakdownIngredient.Count());
                ingredientsBrokenDownList.AddRange(breakdownIngredient);
            }
            else
            {
                _log.LogDebug("Did not find any items to break down item ID {ItemID}", itemID);
                ingredientsBrokenDownList.Add(ingredient);
            }
        }
        _log.LogInformation(
            "Breakdown complete. {IngCount} ingredients returned",
            ingredientList.Count
        );
        return ingredientsBrokenDownList;
    }

    public IEnumerable<IngredientPoco> BreakdownItem(int itemID)
    {
        _log.LogInformation("Fetching recipes for item ID {ItemID} from gateway", itemID);
        var ingredientRecipes = _recipes.GetRecipesForItem(itemID);
        _log.LogInformation("No recipe was found for item ID {ItemID} ", itemID);

        foreach (var ingredientRecipe in ingredientRecipes)
        {
            if (ingredientRecipe is null)
                continue;

            var ingredientRecipeID = ingredientRecipe.RecipeID;
            if (CanMakeRecipe(ingredientRecipeID))
            {
                var recipeIngredients = BreakdownRecipe(ingredientRecipeID);
                foreach (var ingredient in recipeIngredients)
                {
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
