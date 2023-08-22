using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using GilGoblin.Exceptions;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using GilGoblin.Services;

namespace GilGoblin.Crafting;

public class CraftingCalculator : ICraftingCalculator
{
    private readonly IRecipeRepository _recipes;
    private readonly IPriceRepository<PricePoco> _prices;
    private readonly IRecipeCostRepository _recipeCosts;
    private readonly IRecipeGrocer _grocer;
    private readonly ILogger<CraftingCalculator> _logger;
    public static int ERROR_DEFAULT_COST { get; } = int.MaxValue;

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

    public async Task<(int, int)> CalculateCraftingCostForItem(int worldID, int itemID)
    {
        var errorReturn = (-1, ERROR_DEFAULT_COST);
        if (worldID < 1 || itemID < 1)
            return errorReturn;

        var recipes = _recipes.GetRecipesForItem(itemID);
        if (!recipes.Any())
            return errorReturn;

        var (recipeID, lowestCraftingCost) = await GetLowestCraftingCost(worldID, recipes);
        LogCraftingResult(worldID, itemID, recipes.Count(), lowestCraftingCost);
        return (recipeID, lowestCraftingCost);
    }

    public async Task<int> CalculateCraftingCostForRecipe(int worldID, int recipeID)
    {
        var calculated = await _recipeCosts.Get(worldID, recipeID);
        if (calculated is not null)
            return calculated.Cost;

        try
        {
            var recipe = _recipes.Get(recipeID);
            var ingredients = await _grocer.BreakdownRecipeById(recipeID);
            if (recipe is null || ingredients is null || !ingredients.Any())
                return ERROR_DEFAULT_COST;

            var ingredientPrices = GetIngredientPrice(worldID, recipe.TargetItemID, ingredients);
            var craftIngredients = AddPricesToIngredients(ingredients, ingredientPrices);

            var craftingCost = await CalculateCraftingCostForIngredients(worldID, craftIngredients);
            var lastUpdated = ingredientPrices
                .FirstOrDefault()
                .LastUploadTime.ConvertLongUnixMsToDateTime();
            await _recipeCosts.Add(
                new RecipeCostPoco
                {
                    RecipeID = recipeID,
                    WorldID = worldID,
                    Cost = craftingCost,
                    Created = DateTimeOffset.Now,
                    Updated = lastUpdated
                }
            );
            return craftingCost;
        }
        catch (DataNotFoundException)
        {
            _logger.LogError(
                $"Failed to find market data while calculating crafting cost for recipe {recipeID} in world {worldID}"
            );
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to calculate crafting cost: {e.Message}");
        }
        return ERROR_DEFAULT_COST;
    }

    public async Task<int> CalculateCraftingCostForIngredients(
        int worldID,
        IEnumerable<CraftIngredientPoco> craftIngredients
    )
    {
        var totalCraftingCost = 0;
        foreach (var craft in craftIngredients)
        {
            var (_, craftingCost) = await CalculateCraftingCostForItem(worldID, craft.ItemID);
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
                var market = price.First(e => e.ItemID == ingredient.ItemID);
                crafts.Add(new CraftIngredientPoco(ingredient, market));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Failed to match market prices to ingredients: {ex.Message}");
        }
        return crafts;
    }

    private IEnumerable<PricePoco> GetIngredientPrice(
        int worldID,
        int itemID,
        IEnumerable<IngredientPoco> ingredients
    )
    {
        var itemIDList = ingredients.Where(i => i is not null).Select(e => e!.ItemID).ToList();
        itemIDList.Add(itemID);
        itemIDList.Sort();

        var result = new List<PricePoco>();
        foreach (var ingredientID in itemIDList)
        {
            var ingredientPrice = _prices.Get(worldID, ingredientID);
            if (ingredientPrice is not null)
                result.Add(ingredientPrice);
        }

        if (!result.Any())
            throw new DataNotFoundException();

        return result;
    }

    private async Task<(int, int)> GetLowestCraftingCost(
        int worldID,
        IEnumerable<RecipePoco?> recipes
    )
    {
        var lowestCost = ERROR_DEFAULT_COST;
        var recipeId = -1;
        foreach (var recipe in recipes.Where(recipe => recipe is not null))
        {
            var recipeCost = ERROR_DEFAULT_COST;
            var cached = await _recipeCosts.Get(worldID, recipe.ID);
            if (cached is not null)
                recipeCost = cached.Cost;
            else
            {
                recipeCost = await CalculateCraftingCostForRecipe(worldID, recipe.ID);
                if (ERROR_DEFAULT_COST - recipeCost < 1000)
                    continue;

                await _recipeCosts.Add(
                    new RecipeCostPoco
                    {
                        RecipeID = recipe.ID,
                        WorldID = worldID,
                        Cost = recipeCost,
                        Created = DateTimeOffset.Now,
                        Updated = DateTimeOffset.Now,
                    }
                );
            }

            if (recipeCost < lowestCost)
            {
                lowestCost = recipeCost;
                recipeId = recipe.ID;
            }
        }
        return (recipeId, lowestCost);
    }

    private void LogCraftingResult(int worldID, int itemID, int recipeCount, float craftingCost)
    {
        if (craftingCost >= (ERROR_DEFAULT_COST - 1000))
            LogErrorCraftingCostForItem(worldID, itemID, recipeCount);
        else
            LogSucessInfo(worldID, itemID, recipeCount, craftingCost);
    }

    private void LogSucessInfo(int worldID, int itemID, int recipeCount, float craftingCost)
    {
        _logger.LogInformation(
            "Successfully calculated crafting cost of {LowestCost} "
                + "for item {ItemID} world {WorldID} with {RecipeCount} craftable recipes",
            craftingCost,
            itemID,
            worldID,
            recipeCount
        );
    }

    private void LogErrorCraftingCostForItem(int worldID, int ingredientID, int recipesCount)
    {
        _logger.LogError(
            "Failed to calculate crafting cost of: world {worldID}, item {itemID} despite having {count} recipes",
            worldID,
            ingredientID,
            recipesCount
        );
    }
}
