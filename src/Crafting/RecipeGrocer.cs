using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using GilGoblin.Extensions;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Crafting;

public class RecipeGrocer : IRecipeGrocer
{
    private readonly IRecipeRepository _recipes;
    private readonly ILogger<RecipeGrocer> _logger;

    public RecipeGrocer(IRecipeRepository recipes, ILogger<RecipeGrocer> logger)
    {
        _recipes = recipes;
        _logger = logger;
    }

    public async Task<IEnumerable<IngredientPoco?>> BreakdownRecipeById(int recipeID)
    {
        var recipe = _recipes.Get(recipeID);
        if (recipe is null)
        {
            _logger.LogInformation("No recipe was found with ID {RecipeID} ", recipeID);
            return Array.Empty<IngredientPoco>();
        }

        return await BreakdownRecipe(recipe);
    }

    public async Task<IEnumerable<IngredientPoco?>> BreakdownRecipe(RecipePoco recipe) =>
        await BreakDownIngredientEntirely(recipe.GetActiveIngredients());

    public async Task<IEnumerable<IngredientPoco?>> BreakDownIngredientEntirely(
        IEnumerable<IngredientPoco?> ingredientList
    )
    {
        var ingredientsBrokenDownList = new List<IngredientPoco>();
        _logger.LogInformation(
            "Breaking down {IngCount} ingredients in ingredient list",
            ingredientList.Count()
        );
        foreach (var ingredient in ingredientList)
        {
            if (ingredient is null)
                continue;

            var itemID = ingredient.ItemID;
            _logger.LogDebug("Breaking down item ID {ItemID}", itemID);

            var breakdownIngredients = await BreakdownItem(itemID);
            if (breakdownIngredients.Any(i => i is not null && i.Quantity > 0))
            {
                _logger.LogDebug("Found {IngCount} ingredients", breakdownIngredients.Count());
                var ingredients = breakdownIngredients
                    .Where(i => i is not null && i.Quantity > 0)
                    .ToList<IngredientPoco>();
                ingredients.ForEach(i => i.Quantity *= ingredient.Quantity);
                ingredientsBrokenDownList.AddRange(ingredients);
            }
            else
            {
                _logger.LogDebug("Did not find any items to break down item ID {ItemID}", itemID);
                ingredientsBrokenDownList.Add(ingredient);
            }
        }
        _logger.LogInformation(
            "Breakdown complete. {IngCount} ingredients returned",
            ingredientList.Count()
        );
        return ingredientsBrokenDownList;
    }

    public async Task<IEnumerable<IngredientPoco>?> BreakdownItem(int itemID)
    {
        _logger.LogInformation("Fetching recipes for item ID {ItemID} from repo", itemID);
        var ingredientRecipes = _recipes.GetRecipesForItem(itemID);
        _logger.LogInformation("No recipe was found for item ID {ItemID} ", itemID);

        foreach (var ingredientRecipe in ingredientRecipes.Where(ing => ing is not null))
        {
            var ingredientRecipeID = ingredientRecipe.ID;
            if (CanMakeRecipe(ingredientRecipeID))
            {
                var recipeIngredients = await BreakdownRecipeById(ingredientRecipeID);
                var slimList = recipeIngredients.Where(ing => ing is not null && ing.Quantity is 0);

                foreach (var ingredient in slimList)
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
