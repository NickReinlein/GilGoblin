using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Pocos;
using GilGoblin.Repository;
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
        return recipe is null ? Array.Empty<IngredientPoco>() : await BreakdownRecipe(recipe);
    }

    public async Task<IEnumerable<IngredientPoco?>> BreakdownRecipe(RecipePoco recipe) =>
        await BreakDownIngredientEntirely(recipe.GetActiveIngredients());

    public async Task<IEnumerable<IngredientPoco?>> BreakDownIngredientEntirely(
        IEnumerable<IngredientPoco?> ingredientList
    )
    {
        var ingredientsBrokenDownList = new List<IngredientPoco>();
        foreach (var ingredient in ingredientList.Where(i => i?.Quantity > 0))
        {
            var breakdownIngredients = await BreakdownItem(ingredient.ItemID);
            if (!breakdownIngredients.Any())
            {
                ingredientsBrokenDownList.Add(ingredient);
                continue;
            }

            breakdownIngredients.ToList().ForEach(i => i.Quantity *= ingredient.Quantity);
            ingredientsBrokenDownList.AddRange(breakdownIngredients);
        }
        return ingredientsBrokenDownList;
    }

    public async Task<IEnumerable<IngredientPoco>> BreakdownItem(int itemID)
    {
        var allIngredients = new List<IngredientPoco>();
        var allRecipes = _recipes.GetRecipesForItem(itemID);

        foreach (var recipe in allRecipes.Where(r => r is not null))
        {
            var ingredients = await BreakdownRecipeById(recipe.ID);
            MultiplyQuantityProduced(ingredients, recipe);
            allIngredients.AddRange(ingredients);
        }
        return allIngredients;
    }

    private static void MultiplyQuantityProduced(
        IEnumerable<IngredientPoco> recipeIngredients,
        RecipePoco recipe
    )
    {
        foreach (
            var ingredient in recipeIngredients.Where(
                ing => ing is not null && ing.Quantity is not 0 && ing.RecipeID == recipe.ID
            )
        )
        {
            ingredient.Quantity *= recipe.ResultQuantity;
        }
    }
}
