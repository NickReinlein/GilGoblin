using GilGoblin.web;
using Serilog;

namespace GilGoblin.crafting
{
    public class CraftingCalculator : ICraftingCalculator
    {
        private IRecipeGateway _recipeGateway;
        private IMarketDataGateway _marketDataGateway;
        private ILogger _log;

        public CraftingCalculator()
        {
            _recipeGateway = new RecipeGateway();
            _marketDataGateway = new MarketDataGateway();
            _log = new LoggerConfiguration().WriteTo.Console().CreateLogger();
        }

        public CraftingCalculator(
            IRecipeGateway recipeGateway,
            IMarketDataGateway marketDataGateway,
            ILogger log
        )
        {
            _recipeGateway = recipeGateway;
            _marketDataGateway = marketDataGateway;
            _log = log;
        }

        public static int ERROR_DEFAULT_COST { get; } = -1;

        public IEnumerable<IngredientQty> BreakdownRecipe(int recipeID)
        {
            var ingredientList = new List<IngredientQty>();
            var recipe = _recipeGateway.GetRecipe(recipeID);

            if (recipe is null)
                return Array.Empty<IngredientQty>();

            foreach (var ingredient in recipe.ingredients)
            {
                // if no recipe, can we look it up with the ingredient's ID?
                // TODO ; can use LINQ here.. wait for unit tests
                if (canMakeRecipe(ingredient.recipeID))
                    ingredientList.AddRange(BreakdownRecipe(ingredient.recipeID));
            }
            return ingredientList;
        }

        private bool canMakeRecipe(int recipeID)
        {
            //add functionality here to check for crafting levels per recipe
            return recipeID is not 0;
        }

        public int CalculateCraftingCost(int worldID, int itemID)
        {
            try
            {
                var list = new List<int> { itemID };
                var marketData = _marketDataGateway.GetMarketDataItems(worldID, list);

                if (!marketData.Any())
                    throw new MarketDataNotFoundException();

                float itemCost = (float)marketData.First().averageSold;
                if (itemCost < 1)
                    throw new MarketDataNotFoundException(COST_MISSING_ERROR);

                LogMarketDataSuccess(worldID, itemID);
                int craftingCost = (int)MathF.Floor(itemCost);

                LogCraftingCostSuccess(worldID, itemID, craftingCost);
                return craftingCost;
            }
            catch (MarketDataNotFoundException err)
            {
                LogMarketDataNotFoundError(worldID, itemID, err);
                return ERROR_DEFAULT_COST;
            }
        }

        private static void LogCraftingCostSuccess(int worldID, int itemID, int craftingCost)
        {
            Log.Information(
                $"Returning crafitng cost for itemID: {itemID}, worldID: {worldID}, craftingCost: {craftingCost}",
                itemID,
                worldID,
                craftingCost
            );
        }

        private static void LogMarketDataSuccess(int worldID, int itemID)
        {
            Log.Information(
                $"Found the market data for itemID: {itemID}, worldID: {worldID}.",
                itemID,
                worldID
            );
        }

        private static void LogMarketDataNotFoundError(
            int worldID,
            int itemID,
            MarketDataNotFoundException err
        )
        {
            Log.Error(
                $"Failed to find market data for itemID: {itemID}, worldID: {worldID}. Error Message: {err}",
                itemID,
                worldID,
                err.Message
            );
        }

        private const string COST_MISSING_ERROR =
            "Found the item but no value for averageSale, used to calculate cost.";
    }
}
