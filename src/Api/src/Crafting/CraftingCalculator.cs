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

        var recipes = _recipes.GetRecipesForItem(itemId).ToList();
        if (!recipes.Any())
            return errorReturn;

        var (recipeId, lowestCraftingCost) = await GetLowestCraftingCost(worldId, recipes);
        LogCraftingResult(worldId, itemId, recipes.Count, lowestCraftingCost);
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

            var ingredientPrices = GetIngredientPrices(worldId, recipe.TargetItemId, ingredients).ToList();
            if (!ingredientPrices.Any())
                return ERROR_DEFAULT_COST;

            var craftingCost = await CalculateCraftingCostFromIngredients(worldId, ingredients, ingredientPrices);

            if (craftingCost <= 1)
                throw new ArithmeticException(
                    $"Failed to calculate crafting cost for ingredients of recipe {recipeId} in world {worldId}");
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

            if (craftingCost <= 1)
                throw new ArgumentException(
                    $"Crafting cost cannot be {craftingCost} for recipe {recipeId} in world {worldId}");

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
        var ingredientList = ingredients.ToList();

        var craftIngredients = AddPricesToIngredients(ingredientList, ingredientPrices);
        if (!craftIngredients.Any())
        {
            _logger.LogError(
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
                    .First(e =>
                        e.ItemId == ingredient.ItemId);
                crafts.Add(new CraftIngredientPoco(ingredient, market));
            }

            return crafts;
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to match market prices to ingredients: {ex.Message}");
            return new List<CraftIngredientPoco>();
        }
    }

    public IEnumerable<PricePoco> GetIngredientPrices(
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
            else
                _logger.LogError($"No price found for ingredient {ingredientId} in world {worldId}");
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
        {
            var message =
                $"Failed to calculate crafting cost of: world {worldId}, item {itemId} with {recipeCount} recipes";
            _logger.LogError(message);
        }
        else
        {
            var message =
                $"Successfully calculated crafting cost of {craftingCost} for item {itemId} world {worldId} with {recipeCount} craftable recipes";
            _logger.LogInformation(message);
        }
    }
}