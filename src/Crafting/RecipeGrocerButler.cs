using GilGoblin.Pocos;
using GilGoblin.Web;
using Serilog;

namespace GilGoblin.Crafting;

public class RecipeGrocerButler : IRecipeGrocer
{
    private readonly IRecipeGateway _gateway;
    private readonly ILogger _log;

    public RecipeGrocerButler(IRecipeGateway gateway, ILogger log)
    {
        _gateway = gateway;
        _log = log;
    }

    public IEnumerable<IngredientPoco> BreakdownRecipe(int recipeID)
    {
        var ingredientList = new List<IngredientPoco>();
        _log.Information("Fetching recipe ID {RecipeID} from gateway", recipeID);
        var recipe = _gateway.GetRecipe(recipeID);
        if (recipe is null)
        {
            _log.Information("No recipe was found with ID {RecipeID} ", recipeID);
            return Array.Empty<IngredientPoco>();
        }

        BreakDownIngredient(ingredientList);

        return ingredientList;
    }

    public List<IngredientPoco> BreakDownIngredient(List<IngredientPoco> ingredientList)
    {
        _log.Information("Breaking down {IngCount} ingredients in ingredient list", ingredientList.Count);
        foreach (var ingredient in ingredientList)
        {
            var itemID = ingredient.ItemID;
            _log.Debug("Breaking down item ID {ItemID}", itemID);
            var breakdownIngredient = BreakdownItem(itemID);
            if (breakdownIngredient.Any())
            {
                _log.Debug("Found {IngCount} ingredients", breakdownIngredient.Count());
                ingredientList.AddRange(breakdownIngredient);
            }
            else
            {
                _log.Debug("Did not find any items to break down item ID {ItemID}", itemID);
                ingredientList.Add(ingredient);
            }
        }
        _log.Information("Breakdown complete. {IngCount} ingredients returned", ingredientList.Count);
        return ingredientList;
    }

    public IEnumerable<IngredientPoco> BreakdownItem(int itemID)
    {
        _log.Information("Fetching recipes for item ID {ItemID} from gateway", itemID);
        var ingredientRecipes = _gateway.GetRecipesForItem(itemID);
        _log.Information("No recipe was found for item ID {ItemID} ", itemID);

        foreach (var ingredientRecipe in ingredientRecipes)
        {
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
