using GilGoblin.Pocos;
using GilGoblin.Repository;
using GilGoblin.Web;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GilGoblin.Crafting;

public class CraftingCalculator : ICraftingCalculator
{
    private readonly Repository.IRecipeRepository _recipes;
    private readonly IPriceRepository _prices;
    private readonly IRecipeGrocer _grocer;
    private readonly ILogger<CraftingCalculator> _log;
    public static int ERROR_DEFAULT_COST { get; } = int.MaxValue;

    public CraftingCalculator(
        Repository.IRecipeRepository recipes,
        IPriceRepository prices,
        IRecipeGrocer grocer,
        ILogger<CraftingCalculator> log
    )
    {
        _recipes = recipes;
        _prices = prices;
        _grocer = grocer;
        _log = log;
    }

    public int CalculateCraftingCostForItem(int worldID, int itemID)
    {
        var recipes = _recipes.GetRecipesForItem(itemID);
        var recipeCount = recipes.Count();
        var craftingCost = GetLowestCraftingCost(worldID, recipes);
        LogCraftingResult(worldID, itemID, recipeCount, craftingCost);
        return craftingCost;
    }

    public int CalculateCraftingCostForRecipe(int worldID, int recipeID)
    {
        try
        {
            var recipe = this._recipes.Get(recipeID);
            var ingredients = _grocer.BreakdownRecipe(recipeID);
            if (recipe is null || !ingredients.Any())
                return ERROR_DEFAULT_COST;

            var ingredientsMarketData = GetIngredientMarketData(
                worldID,
                recipe.TargetItemID,
                ingredients
            );
            IEnumerable<CraftIngredientPoco> craftIngredients = AddMarketDataToIngredients(
                ingredients,
                ingredientsMarketData
            );

            var craftingCost = CalculateCraftingCostForIngredients(worldID, craftIngredients);

            Log.Information(
                "Successfully calculated crafting cost of {CraftCost} for recipe {RecipeID} world {WorldID} with {IngCount} ingredients",
                craftingCost,
                recipeID,
                worldID,
                ingredients.Count()
            );
            return craftingCost;
        }
        catch (MarketDataNotFoundException)
        {
            Log.Error(
                "Failed to find market data while calculating crafting cost for recipe {RecipeID} in world {WorldID} for item {ItemID}",
                recipeID,
                worldID
            );
            return ERROR_DEFAULT_COST;
        }
        catch (Exception e)
        {
            Log.Error("Failed to calculate crafting cost: {e}", e.Message);
            return ERROR_DEFAULT_COST;
        }
    }

    public int CalculateCraftingCostForIngredients(
        int worldID,
        IEnumerable<CraftIngredientPoco> craftIngredients
    )
    {
        var totalCraftingCost = 0;
        foreach (var craft in craftIngredients)
        {
            var averageSold = (int)craft.MarketData.AverageSold;
            var craftingCost = CalculateCraftingCostForItem(worldID, craft.ItemID);
            var minCost = Math.Min(averageSold, craftingCost);

            Log.Information(
                "Calculated cost {MinCost} for {ItemID} in world {WorldID}, based on sell price {Sold} and crafting cost {CraftCost}",
                minCost,
                craft.ItemID,
                worldID,
                averageSold,
                craftingCost
            );
            totalCraftingCost += craft.Quantity * minCost;
        }

        return totalCraftingCost;
    }

    public static List<CraftIngredientPoco> AddMarketDataToIngredients(
        IEnumerable<IngredientPoco> ingredients,
        IEnumerable<MarketDataPoco> marketData
    )
    {
        List<CraftIngredientPoco> crafts = new();
        foreach (var ingredient in ingredients)
        {
            var market = marketData.First(e => e.ItemID == ingredient.ItemID);
            if (market is null)
                throw new MarketDataNotFoundException();

            crafts.Add(new CraftIngredientPoco(ingredient, market));
        }
        return crafts;
    }

    private IEnumerable<MarketDataPoco> GetIngredientMarketData(
        int worldID,
        int itemID,
        IEnumerable<IngredientPoco> ingredients
    )
    {
        var itemIDList = ingredients.Select(e => e.ItemID).ToList();
        itemIDList.Add(itemID);
        itemIDList.Sort();
        var result = new List<MarketDataPoco>();
        foreach (var ingredientID in itemIDList)
        {
            result.Add(_prices.Get(worldID, ingredientID));
        }

        if (!result.Any())
            throw new MarketDataNotFoundException();
        return result;
    }

    private int GetLowestCraftingCost(int worldID, IEnumerable<RecipePoco> recipes)
    {
        var lowestCost = ERROR_DEFAULT_COST;
        foreach (var recipe in recipes)
        {
            var recipeCost = CalculateCraftingCostForRecipe(worldID, recipe.RecipeID);
            lowestCost = Math.Min(recipeCost, lowestCost);
        }
        return lowestCost;
    }

    private void LogCraftingResult(int worldID, int itemID, int recipeCount, int craftingCost)
    {
        if (craftingCost >= (ERROR_DEFAULT_COST - 1000))
            LogErrorCraftingCostForItem(worldID, itemID, recipeCount);
        else
            LogSucessInfo(worldID, itemID, recipeCount, craftingCost);
    }

    private static void LogSucessInfo(int worldID, int itemID, int recipeCount, int craftingCost)
    {
        Log.Information(
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
        _log.LogError(
            "Failed to calculate crafting cost of: world {worldID}, item {itemID} despite having {count} recipes",
            worldID,
            ingredientID,
            recipesCount
        );
    }
}
