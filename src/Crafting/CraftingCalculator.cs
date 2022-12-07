using GilGoblin.Pocos;
using GilGoblin.Web;
using Microsoft.Extensions.Logging;
using Serilog;

namespace GilGoblin.Crafting;

public class CraftingCalculator : ICraftingCalculator
{
    private readonly IRecipeGateway _recipeGateway;
    private readonly IMarketDataGateway _marketGateway;
    private readonly IRecipeGrocer _grocer;
    private readonly ILogger<CraftingCalculator> _log;
    public static int ERROR_DEFAULT_COST { get; } = int.MaxValue;

    public CraftingCalculator(
        IRecipeGateway recipeGateway,
        IMarketDataGateway marketDataGateway,
        IRecipeGrocer grocer,
        ILogger<CraftingCalculator> log
    )
    {
        _recipeGateway = recipeGateway;
        _marketGateway = marketDataGateway;
        _log = log;
        _grocer = grocer;
    }

    public int CalculateCraftingCostForItem(int worldID, int itemID)
    {
        var recipes = _recipeGateway.GetRecipesForItem(itemID);
        var recipeCount = recipes.Count();
        var craftingCost = GetLowestCraftingCost(worldID, recipes);

        if (craftingCost >= (ERROR_DEFAULT_COST - 1000))
            LogErrorCraftingCostForItem(worldID, itemID, recipeCount);
        else
            LogSucessInfo(worldID, itemID, recipeCount, craftingCost);
        return craftingCost;
    }


    public int CalculateCraftingCostForRecipe(int worldID, int recipeID)
    {
        try
        {
            var recipe = _recipeGateway.GetRecipe(recipeID);
            var ingredients = _grocer.BreakdownRecipe(recipeID);
            if (recipe is null || !ingredients.Any()) return ERROR_DEFAULT_COST;

            var ingredientsMarketData = GetIngredientMarketData(worldID, recipe.TargetItemID, ingredients);
            IEnumerable<CraftIngredient> craftIngredients = AddMarketDataToIngredients(ingredients, ingredientsMarketData);

            var craftingCost = CalculateCraftingCostForIngredients(worldID, craftIngredients);

            Log.Information("Successfully calculated crafting cost of {CraftCost} for recipe {RecipeID} world {WorldID} with {IngCount} ingredients",
                craftingCost, recipeID, worldID, ingredients.Count());
            return craftingCost;
        }
        catch (MarketDataNotFoundException)
        {
            Log.Error("Failed to find market data while calculating crafting cost for recipe {RecipeID} in world {WorldID} for item {ItemID}", recipeID, worldID);
            return ERROR_DEFAULT_COST;
        }
        catch (Exception e)
        {
            Log.Error("Failed to calculate crafting cost: {e}", e.Message);
            return ERROR_DEFAULT_COST;
        }
    }

    public int CalculateCraftingCostForIngredients(int worldID, IEnumerable<CraftIngredient> craftIngredients)
    {
        var totalCraftingCost = 0;
        foreach (var craft in craftIngredients)
        {
            var averageSold = (int)craft.MarketData.AverageSold;
            var craftingCost = CalculateCraftingCostForItem(worldID, craft.ItemID);
            var minCost = Math.Min(averageSold, craftingCost);

            Log.Information("Calculated cost {MinCost} for {ItemID} in world {WorldID}, based on sell price {Sold} and crafting cost {CraftCost}",
                minCost, craft.ItemID, worldID, averageSold, craftingCost);
            totalCraftingCost += craft.Quantity * minCost;
        }

        return totalCraftingCost;
    }

    public static List<CraftIngredient> AddMarketDataToIngredients(IEnumerable<IngredientPoco> ingredients, IEnumerable<MarketDataPoco> marketData)
    {
        List<CraftIngredient> crafts = new();
        foreach (var ingredient in ingredients)
        {
            var market = marketData.Single(e => e.ItemID == ingredient.ItemID);
            if (market is null) throw new MarketDataNotFoundException();

            crafts.Add(new CraftIngredient(ingredient, market));
        }
        return crafts;
    }

    private IEnumerable<MarketDataPoco> GetIngredientMarketData(int worldID, int itemID, IEnumerable<IngredientPoco> ingredients)
    {
        var itemIDList = ingredients.Select(e => e.ItemID).ToList();
        itemIDList.Add(itemID);
        itemIDList.Sort();
        var marketData = _marketGateway.GetMarketData(worldID, itemIDList);
        if (!marketData.Any()) throw new MarketDataNotFoundException();
        return marketData;
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

    private static void LogSucessInfo(int worldID, int itemID, int recipeCount, int craftingCost)
    {
        Log.Information("Successfully calculated crafting cost of {LowestCost} " +
                            "for item {ItemID} world {WorldID} with {RecipeCount} craftable recipes",
                            craftingCost, itemID, worldID, recipeCount);
    }

    private void LogErrorCraftingCostForItem(int worldID, int itemID, int recipesCount)
    {
        _log.LogError("Failed to calculate crafting cost of: world {worldID}, item {itemID} despite having {count} recipes", worldID, itemID, recipesCount);
    }
}
