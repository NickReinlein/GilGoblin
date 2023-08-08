using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Exceptions;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Crafting;

public class CraftingCalculator : ICraftingCalculator
{
    private readonly IRecipeRepository _recipes;
    private readonly IPriceRepository<PricePoco> _prices;
    private readonly IRecipeGrocer _grocer;
    private readonly ILogger<CraftingCalculator> _logger;
    public static int ERROR_DEFAULT_COST { get; } = int.MaxValue;

    public CraftingCalculator(
        IRecipeRepository recipes,
        IPriceRepository<PricePoco> prices,
        IRecipeGrocer grocer,
        ILogger<CraftingCalculator> logger
    )
    {
        _recipes = recipes;
        _prices = prices;
        _grocer = grocer;
        _logger = logger;
    }

    public async Task<(int, float)> CalculateCraftingCostForItem(int worldID, int itemID)
    {
        var recipes = _recipes.GetRecipesForItem(itemID);
        var (recipeID, lowestCraftingCost) = await GetLowestCraftingCost(worldID, recipes);
        LogCraftingResult(worldID, itemID, recipes.Count(), lowestCraftingCost);
        return (recipeID, lowestCraftingCost);
    }

    public async Task<int> CalculateCraftingCostForRecipe(int worldID, int recipeID)
    {
        try
        {
            var recipe = _recipes.Get(recipeID);
            var ingredients = await _grocer.BreakdownRecipeById(recipeID);
            if (recipe is null || ingredients is null || !ingredients.Any())
                return ERROR_DEFAULT_COST;

            var ingredientPrices = GetIngredientPrice(worldID, recipe.TargetItemID, ingredients);
            var craftIngredients = AddPricesToIngredients(ingredients, ingredientPrices);

            var craftingCost = await CalculateCraftingCostForIngredients(worldID, craftIngredients);

            _logger.LogInformation(
                "Successfully calculated crafting cost of {CraftCost} for recipe {RecipeID} world {WorldID} with {IngCount} ingredients",
                craftingCost,
                recipeID,
                worldID,
                ingredients.Count()
            );
            return craftingCost;
        }
        catch (DataNotFoundException)
        {
            _logger.LogError(
                $"Failed to find market data while calculating crafting cost for recipe {recipeID} in world {worldID}"
            );
            return ERROR_DEFAULT_COST;
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to calculate crafting cost: {e.Message}");
            return ERROR_DEFAULT_COST;
        }
    }

    public async Task<int> CalculateCraftingCostForIngredients(
        int worldID,
        IEnumerable<CraftIngredientPoco> craftIngredients
    )
    {
        var totalCraftingCost = 0f;
        foreach (var craft in craftIngredients)
        {
            var averageSold = craft.Price.AverageSold;
            var (_, craftingCost) = await CalculateCraftingCostForItem(worldID, craft.ItemID);
            var minCost = Math.Min(averageSold, craftingCost);

            _logger.LogInformation(
                "Calculated cost {MinCost} for {ItemID} in world {WorldID}, based on sell price {Sold} and crafting cost {CraftCost}",
                minCost,
                craft.ItemID,
                worldID,
                averageSold,
                craftingCost
            );
            totalCraftingCost += craft.Quantity * minCost;
        }

        return (int)totalCraftingCost;
    }

    public static List<CraftIngredientPoco> AddPricesToIngredients(
        IEnumerable<IngredientPoco> ingredients,
        IEnumerable<PricePoco> price
    )
    {
        List<CraftIngredientPoco> crafts = new();
        foreach (var ingredient in ingredients)
        {
            var market = price.First(e => e.ItemID == ingredient.ItemID);
            crafts.Add(new CraftIngredientPoco(ingredient, market));
        }
        return crafts;
    }

    private IEnumerable<PricePoco> GetIngredientPrice(
        int worldID,
        int itemID,
        IEnumerable<IngredientPoco> ingredients
    )
    {
        var itemIDList = ingredients
            .Where(i => i is not null)
            .ToList()
            .Select(e => e!.ItemID)
            .ToList();
        itemIDList.Add(itemID);
        itemIDList.Sort();
        var result = new List<PricePoco>();
        foreach (var ingredientID in itemIDList)
        {
            var ingredientPrice = _prices.Get(worldID, ingredientID);
            if (ingredientPrice is null)
                continue;
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
        foreach (var recipe in recipes)
        {
            if (recipe is null)
                continue;
            var recipeCost = await CalculateCraftingCostForRecipe(worldID, recipe.ID);
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
