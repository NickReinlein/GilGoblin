using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using GilGoblin.Api.Repository;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Api.Crafting;

public class RecipeGrocer : IRecipeGrocer
{
    private readonly IRecipeRepository _recipes;
    private readonly ILogger<RecipeGrocer> _logger;

    public RecipeGrocer(IRecipeRepository recipes, ILogger<RecipeGrocer> logger)
    {
        _recipes = recipes;
        _logger = logger;
    }

    public async Task<IEnumerable<IngredientPoco>> BreakdownRecipeById(int recipeId)
    {
        var failure = Array.Empty<IngredientPoco>();
        try
        {
            var recipe = _recipes.Get(recipeId);
            if (recipe is null)
                return failure;

            return await BreakdownRecipe(recipe);
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to break down ingredients for recipe {recipeId}: {e.Message}");
            return failure;
        }
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
            var breakdown = await BreakdownItem(ingredient.ItemId);
            var breakdownIngredients = breakdown.ToList();
            if (!breakdownIngredients.Any())
            {
                ingredientsBrokenDownList.Add(ingredient);
                continue;
            }

            breakdownIngredients.ForEach(i => i.Quantity *= ingredient.Quantity);
            ingredientsBrokenDownList.AddRange(breakdownIngredients);
        }

        return ingredientsBrokenDownList;
    }

    public async Task<IEnumerable<IngredientPoco>> BreakdownItem(int itemId)
    {
        var allIngredients = new List<IngredientPoco>();
        var allRecipes = _recipes.GetRecipesForItem(itemId);

        foreach (var recipe in allRecipes.Where(r => r is not null))
        {
            var ingredients = await BreakdownRecipeById(recipe.Id);
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
                ing => ing is not null && ing.Quantity is not 0 && ing.RecipeId == recipe.Id
            )
        )
        {
            ingredient.Quantity *= recipe.ResultQuantity;
        }
    }
}