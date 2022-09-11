using GilGoblin.Pocos;
using GilGoblin.Web;
using Serilog;

namespace GilGoblin.Ext;

public static class RecipeGatewayExtension : IRecipeGatewayExtension
{
    public static IEnumerable<IngredientPoco> BreakdownRecipe(this RecipeGateway gateway, int recipeID)
    {
        var ingredientList = new List<IngredientPoco>();

        var recipe = gateway.GetRecipe(recipeID);
        if (recipe is null) return Array.Empty<IngredientPoco>();

        foreach (var ingredient in recipe.Ingredients)
        {
            var breakdownIngredient = gateway.BreakdownItem(ingredient.ItemID);
            if (breakdownIngredient.Any())
                ingredientList.AddRange(breakdownIngredient);
            else
                ingredientList.Add(ingredient);
        }
        return ingredientList;
    }

    public static IEnumerable<IngredientPoco> BreakdownItem(this RecipeGateway gateway, int itemID)
    {
        var ingredientRecipes = gateway.GetRecipesForItem(itemID);

        foreach (var ingredientRecipe in ingredientRecipes)
        {
            var ingredientRecipeID = ingredientRecipe.RecipeID;
            if (CanMakeRecipe(ingredientRecipeID))
            {
                var recipeIngredients = gateway.BreakdownRecipe(ingredientRecipeID);
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
