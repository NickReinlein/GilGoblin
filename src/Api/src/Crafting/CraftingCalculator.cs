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

public interface ICraftingCalculator
{
    Task<int> CalculateCraftingCostForRecipe(int worldId, int recipeId, bool isHq);
}

public class CraftingCalculator(
    IRecipeRepository recipeRepo,
    IPriceRepository<PricePoco> priceRepo,
    IRecipeCostRepository recipeCostsRepo,
    IRecipeGrocer recipeGrocer,
    ILogger<CraftingCalculator> logger)
    : ICraftingCalculator
{
    private const int hoursBeforeDataExpiry = 96;
    public static readonly int ErrorDefaultCost = int.MaxValue / 2;

    public async Task<(int, int)> CalculateCraftingCostForItem(int worldId, int itemId)
    {
        var errorReturn = (-1, ERROR_DEFAULT_COST: ErrorDefaultCost);
        if (worldId < 1 || itemId < 1)
            return errorReturn;

        var itemRecipes = recipeRepo.GetRecipesForItem(itemId);
        if (!itemRecipes.Any())
            return errorReturn;

        var (recipeId, lowestCraftingCost) = await GetLowestCraftingCost(worldId, itemRecipes);
        LogCraftingErrors(worldId, itemId, itemRecipes.Count, lowestCraftingCost);
        return (recipeId, lowestCraftingCost);
    }

    public async Task<int> CalculateCraftingCostForRecipe(int worldId, int recipeId, bool isHq)
    {
        var existing = await recipeCostsRepo.GetAsync(worldId, recipeId, isHq);
        if (existing is not null &&
            existing.LastUpdated >= DateTime.UtcNow.AddHours(-hoursBeforeDataExpiry))
            return existing.Amount;

        try
        {
            var recipe = recipeRepo.Get(recipeId);
            var result = await recipeGrocer.BreakdownRecipeById(recipeId);
            var ingredients = result.ToList();
            if (recipe is null || !ingredients.Any())
                return ErrorDefaultCost;

            var ingredientPrices = GetIngredientPrices(worldId, ingredients).ToList();

            return await CalculateCraftingCostFromIngredients(worldId, ingredients, ingredientPrices);
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
            return ErrorDefaultCost;
        }

        var craftingCost = await CalculateCraftingCostForIngredients(worldId, craftIngredients);
        return craftingCost;
    }

    private async Task<int> CalculateCraftingCostForIngredients(
        int worldId,
        IEnumerable<CraftIngredientPoco> craftIngredients
    )
    {
        var totalCraftingCost = 0;
        foreach (var craft in craftIngredients)
        {
            // Compare crafting the ingredient vs purchasing it on the market board
            var bestPriceCost = craft.Price.GetBestPriceAmount();
            var (_, craftingCost) = await CalculateCraftingCostForItem(worldId, craft.ItemId);
            var minCost = (int)Math.Min(bestPriceCost, craftingCost);
            totalCraftingCost += craft.Quantity * minCost;
        }

        return totalCraftingCost;
    }

    private List<CraftIngredientPoco> AddPricesToIngredients(
        IEnumerable<IngredientPoco> ingredients,
        IEnumerable<PricePoco> prices
    )
    {
        try
        {
            List<CraftIngredientPoco> crafts = [];
            var uniqueIngredients =
                ingredients
                    .GroupBy(poco => poco.ItemId)
                    .Select(group =>
                        new IngredientPoco { ItemId = group.Key, Quantity = group.Sum(poco => poco.Quantity) })
                    .ToHashSet()
                    .ToList();
            var priceList = prices.ToList();
            foreach (var ingredient in uniqueIngredients)
            {
                var marketPrices = priceList.Where(e =>
                    e.ItemId == ingredient.ItemId).ToList();

                crafts.AddRange(
                    marketPrices.Select(marketPrice =>
                        new CraftIngredientPoco(ingredient, marketPrice)));
            }

            return crafts;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to match market prices to ingredients");
            return [];
        }
    }

    private IEnumerable<PricePoco> GetIngredientPrices(int worldId, IEnumerable<IngredientPoco> ingredients)
    {
        foreach (var ingredient in ingredients)
        {
            var price = priceRepo.Get(worldId, ingredient.ItemId, ingredient.IsHq);
            if (price is not null)
                yield return price;
        }
    }

    private async Task<(int, int)> GetLowestCraftingCost(int worldId, IEnumerable<RecipePoco?> recipeList)
    {
        var lowestCost = ErrorDefaultCost;
        var recipeId = -1;
        foreach (var recipe in recipeList.Where(recipe => recipe is not null))
        {
            var cached = await recipeCostsRepo.GetAsync(worldId, recipe!.Id);

            var recipeCost = cached?.Amount ?? await CalculateCraftingCostForRecipe(worldId, recipe.Id, false);

            if (IsErrorCost(recipeCost))
                throw new ArithmeticException($"Failed to calculate recipe cost for {recipe.Id} in world {worldId}");

            if (recipeCost >= lowestCost)
                continue;

            lowestCost = recipeCost;
            recipeId = recipe.Id;
        }

        return (recipeId, lowestCost);
    }

    private void LogCraftingErrors(int worldId, int itemId, int recipeCount, float craftingCost)
    {
        if (!IsErrorCost(craftingCost))
            return;

        var message =
            $"Failed to calculate crafting cost of: world {worldId}, item {itemId} with {recipeCount} recipes";
        logger.LogError(message);
    }

    private static bool IsErrorCost(float cost) => cost > ErrorDefaultCost || cost < 1;
}