using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Pocos;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.Logging;
using GilGoblin.Api.Repository;

namespace GilGoblin.Api.Crafting;

public class CraftingCalculator(
    IRecipeRepository recipes,
    IPriceRepository<PricePoco> prices,
    IRecipeCostRepository recipeCosts,
    IRecipeGrocer grocer,
    ILogger<CraftingCalculator> logger)
    : ICraftingCalculator
{
    public static readonly int ErrorDefaultCost = int.MaxValue;

    public async Task<(int, int)> CalculateCraftingCostForItem(int worldId, int itemId)
    {
        var errorReturn = (-1, ERROR_DEFAULT_COST: ErrorDefaultCost);
        if (worldId < 1 || itemId < 1)
            return errorReturn;

        var recipes1 = recipes.GetRecipesForItem(itemId);
        if (!recipes1.Any())
            return errorReturn;

        var (recipeId, lowestCraftingCost) = await GetLowestCraftingCost(worldId, recipes1);
        LogCraftingResult(worldId, itemId, recipes1.Count, lowestCraftingCost);
        return (recipeId, lowestCraftingCost);
    }

    public async Task<int> CalculateCraftingCostForRecipe(int worldId, int recipeId)
    {
        var calculated = await recipeCosts.GetAsync(worldId, recipeId);
        if (calculated is not null &&
            calculated.Updated >= DateTime.UtcNow.AddHours(-48))
            return calculated.Cost;

        try
        {
            var recipe = recipes.Get(recipeId);
            var result = await grocer.BreakdownRecipeById(recipeId);
            var ingredients = result.ToList();
            if (recipe is null || !ingredients.Any())
                return ErrorDefaultCost;

            var ingredientPrices = GetIngredientPrices(worldId, recipe.TargetItemId, ingredients).ToList();

            var craftingCost = await CalculateCraftingCostFromIngredients(worldId, ingredients, ingredientPrices);

            if (craftingCost is <= 1 or >= 1147483647)
                throw new ArithmeticException(
                    $"Failed to calculate crafting cost for ingredients of recipe {recipeId} in world {worldId}");

            return craftingCost;
        }
        catch (DataException)
        {
            logger.LogError(
                $"Failed to find market data while calculating crafting cost for recipe {recipeId} in world {worldId}"
            );
        }
        catch (Exception e)
        {
            logger.LogError($"Failed to calculate crafting cost: {e.Message}");
        }

        return ErrorDefaultCost;
    }

    private async Task<int> CalculateCraftingCostFromIngredients(int worldId, IEnumerable<IngredientPoco> ingredients,
        IEnumerable<PricePoco> ingredientPrices)
    {
        var ingredientList = ingredients.ToList();

        var craftIngredients = AddPricesToIngredients(ingredientList, ingredientPrices);
        if (!craftIngredients.Any())
        {
            logger.LogError(
                $"Failed to calculate crafting ingredient prices for world {worldId} for ingredients: {ingredientList.Select(i => i.ItemId)}");
            return 0;
        }

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
        try
        {
            List<CraftIngredientPoco> crafts = new();
            var uniqueIngredients =
                ingredients
                    .GroupBy(poco => poco.ItemId)
                    .Select(group
                        => new IngredientPoco { ItemId = group.Key, Quantity = group.Sum(poco => poco.Quantity) })
                    .ToHashSet()
                    .ToList();
            var priceList = price.ToList();
            foreach (var ingredient in uniqueIngredients)
            {
                var market = priceList
                    .FirstOrDefault(e =>
                        e.ItemId == ingredient.ItemId);
                if (market is not null)
                    crafts.Add(new CraftIngredientPoco(ingredient, market));
            }

            return crafts;
        }
        catch (Exception ex)
        {
            logger.LogError($"Failed to match market prices to ingredients: {ex.Message}");
            return new List<CraftIngredientPoco>();
        }
    }

    public IEnumerable<PricePoco> GetIngredientPrices(
        int worldId,
        int itemId,
        IEnumerable<IngredientPoco> ingredients
    )
    {
        var itemIdList = ingredients.Select(e => e.ItemId).ToList();
        itemIdList.Add(itemId);
        itemIdList.Sort();

        var result = new List<PricePoco>();
        foreach (var ingredientId in itemIdList)
        {
            var ingredientPrice = prices.Get(worldId, ingredientId);
            if (ingredientPrice is not null)
                result.Add(ingredientPrice);
            else
                logger.LogError($"No price found for ingredient {ingredientId} in world {worldId}");
        }

        if (!result.Any())
            throw new DataException($"Failed to find ingredient prices for item {itemId} in world {worldId}");

        return result;
    }

    public async Task<(int, int)> GetLowestCraftingCost(int worldId, IEnumerable<RecipePoco?> recipeList)
    {
        var lowestCost = ErrorDefaultCost;
        var recipeId = -1;
        foreach (var recipe in recipeList.Where(recipe => recipe is not null))
        {
            var cached = await recipeCosts.GetAsync(worldId, recipe!.Id);

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
        if (craftingCost >= ErrorDefaultCost - 10000)
        {
            var message =
                $"Failed to calculate crafting cost of: world {worldId}, item {itemId} with {recipeCount} recipes";
            logger.LogError(message);
        }
        else
        {
            var message =
                $"Successfully calculated crafting cost of {craftingCost} for item {itemId} world {worldId} with {recipeCount} craftable recipes";
            logger.LogInformation(message);
        }
    }
}