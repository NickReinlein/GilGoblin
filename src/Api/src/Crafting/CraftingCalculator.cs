using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.Logging;
using GilGoblin.Api.Repository;

namespace GilGoblin.Api.Crafting;

public class CraftingCalculator : ICraftingCalculator
{
    private readonly IRecipeRepository _recipes;
    private readonly IPriceRepository<PricePoco> _prices;
    private readonly IRecipeCostRepository _recipeCosts;
    private readonly IRecipeGrocer _grocer;
    private readonly ILogger<CraftingCalculator> _logger;
    public static int ERROR_DEFAULT_COST = int.MaxValue;

    public CraftingCalculator(
        IRecipeRepository recipes,
        IPriceRepository<PricePoco> prices,
        IRecipeCostRepository recipeCosts,
        IRecipeGrocer grocer,
        ILogger<CraftingCalculator> logger
    )
    {
        _recipes = recipes;
        _prices = prices;
        _grocer = grocer;
        _recipeCosts = recipeCosts;
        _logger = logger;
    }

    public async Task<(int, int)> CalculateCraftingCostForItem(int worldId, int itemId)
    {
        var errorReturn = (-1, ERROR_DEFAULT_COST);
        if (worldId < 1 || itemId < 1)
            return errorReturn;

        var recipes = _recipes.GetRecipesForItem(itemId);
        if (!recipes.Any())
            return errorReturn;

        var (recipeId, lowestCraftingCost) = await GetLowestCraftingCost(worldId, recipes);
        LogCraftingResult(worldId, itemId, recipes.Count(), lowestCraftingCost);
        return (recipeId, lowestCraftingCost);
    }

    public async Task<int> CalculateCraftingCostForRecipe(int worldId, int recipeId)
    {
        var calculated = await _recipeCosts.GetAsync(worldId, recipeId);
        if (calculated is not null)
            return calculated.Cost;

        try
        {
            var recipe = _recipes.Get(recipeId);
            var result = await _grocer.BreakdownRecipeById(recipeId);
            var ingredients = result?.ToList();
            if (recipe is null || ingredients is null || !ingredients.Any())
                return ERROR_DEFAULT_COST;

            var ingredientPrices = GetIngredientPrice(worldId, recipe.TargetItemId, ingredients).ToList();
            if (!ingredientPrices.Any())
                return ERROR_DEFAULT_COST;

            var craftingCost = await CalculateCraftingCostFromIngredients(worldId, ingredients, ingredientPrices);

            await SaveRecipeCost(worldId, recipeId, ingredientPrices, craftingCost);

            return craftingCost;
        }
        catch (DataException)
        {
            _logger.LogError(
                $"Failed to find market data while calculating crafting cost for recipe {recipeId} in world {worldId}"
            );
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to calculate crafting cost: {e.Message}");
        }

        return ERROR_DEFAULT_COST;
    }

    private async Task SaveRecipeCost(int worldId, int recipeId, List<PricePoco> ingredientPrices, int craftingCost)
    {
        try
        {
            var oldestTime
                = ingredientPrices
                    .Select(l => l.LastUploadTime)
                    .Min();
            var oldestTimestamp = new DateTime(oldestTime).ToUniversalTime();

            await _recipeCosts.Add(
                new RecipeCostPoco
                {
                    RecipeId = recipeId, WorldId = worldId, Cost = craftingCost, Updated = oldestTimestamp
                }
            );
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to add the recipe costs to the recipe cost repository: {e.Message}");
        }
    }

    private async Task<int> CalculateCraftingCostFromIngredients(int worldId, IEnumerable<IngredientPoco> ingredients,
        IEnumerable<PricePoco> ingredientPrices)
    {
        var craftIngredients = AddPricesToIngredients(ingredients, ingredientPrices);
        var craftingCost = await CalculateCraftingCostForIngredients(worldId, craftIngredients);
        return craftingCost;
    }

    public async Task<int> CalculateCraftingCostForIngredients(
        int worldId,
        IEnumerable<CraftIngredientPoco> craftIngredients
    )
    {
        var totalCraftingCost = 0;
        foreach (var craft in craftIngredients)
        {
            var (_, craftingCost) = await CalculateCraftingCostForItem(worldId, craft.ItemId);
            var minCost = (int)Math.Min(craft.Price.AverageSold, craftingCost);
            totalCraftingCost += craft.Quantity * minCost;
        }

        return totalCraftingCost;
    }

    public List<CraftIngredientPoco> AddPricesToIngredients(
        IEnumerable<IngredientPoco> ingredients,
        IEnumerable<PricePoco> price
    )
    {
        List<CraftIngredientPoco> crafts = new();
        try
        {
            foreach (var ingredient in ingredients)
            {
                var market = price.First(e => e.ItemId == ingredient.ItemId);
                crafts.Add(new CraftIngredientPoco(ingredient, market));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to match market prices to ingredients: {ex.Message}");
        }

        return crafts;
    }

    public IEnumerable<PricePoco> GetIngredientPrice(
        int worldId,
        int itemId,
        IEnumerable<IngredientPoco> ingredients
    )
    {
        var itemIdList = ingredients.Where(i => i is not null).Select(e => e!.ItemId).ToList();
        itemIdList.Add(itemId);
        itemIdList.Sort();

        var result = new List<PricePoco>();
        foreach (var ingredientId in itemIdList)
        {
            var ingredientPrice = _prices.Get(worldId, ingredientId);
            if (ingredientPrice is not null)
                result.Add(ingredientPrice);
        }

        if (!result.Any())
            throw new DataException();

        return result;
    }

    public async Task<(int, int)> GetLowestCraftingCost(
        int worldId,
        IEnumerable<RecipePoco?> recipes
    )
    {
        var lowestCost = ERROR_DEFAULT_COST;
        var recipeId = -1;
        foreach (var recipe in recipes.Where(recipe => recipe is not null))
        {
            var cached = await _recipeCosts.GetAsync(worldId, recipe.Id);

            var recipeCost = cached?.Cost ?? await CalculateCraftingCostForRecipe(worldId, recipe.Id);

            if (recipeCost >= lowestCost)
                continue;

            lowestCost = recipeCost;
            recipeId = recipe.Id;
        }

        return (recipeId, lowestCost);
    }

    private void LogCraftingResult(int worldId, int itemId, int recipeCount, float craftingCost)
    {
        if (craftingCost >= (ERROR_DEFAULT_COST - 1000))
            LogErrorCraftingCostForItem(worldId, itemId, recipeCount);
        else
            LogSucessInfo(worldId, itemId, recipeCount, craftingCost);
    }

    private void LogSucessInfo(int worldId, int itemId, int recipeCount, float craftingCost)
    {
        _logger.LogInformation(
            "Successfully calculated crafting cost of {LowestCost} "
            + "for item {ItemId} world {WorldId} with {RecipeCount} craftable recipes",
            craftingCost,
            itemId,
            worldId,
            recipeCount
        );
    }

    private void LogErrorCraftingCostForItem(int worldId, int ingredientId, int recipesCount)
    {
        _logger.LogError(
            "Failed to calculate crafting cost of: world {worldId}, item {itemId} despite having {count} recipes",
            worldId,
            ingredientId,
            recipesCount
        );
    }
}