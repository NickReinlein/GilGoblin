using GilGoblin.pocos;
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

        public IEnumerable<IngredientPoco> BreakdownRecipe(int recipeID)
        {
            var recipe = _recipeGateway.GetRecipe(recipeID);

            if (recipe is null) return Array.Empty<IngredientPoco>();

            var ingredientList = new List<IngredientPoco>();
            foreach (var ingredient in recipe.ingredients)
            {
                bool canCraftIngredient = false;
                var ingredientRecipes = _recipeGateway.GetRecipesForItem(ingredient.ItemID);

                foreach (var ingredientRecipe in ingredientRecipes)
                {
                    int ingredientRecipeID = ingredientRecipe.recipeID;
                    if (_canMakeRecipe(ingredientRecipeID))
                    {
                        var recipeIngredients = BreakdownRecipe(ingredientRecipeID);
                        ingredientList.AddRange(recipeIngredients);
                        canCraftIngredient = true;
                        break;
                        // todo later: each breakdown -> get best price -> choose best breakdwon                        
                    }
                }
                if (!canCraftIngredient)
                {
                    ingredientList.Add(ingredient);
                }
            }
            return ingredientList;
        }

        public int GetBestCostForItem(int worldID, int itemID)
        {
            try
            {
                var list = new List<int> { itemID };
                var marketDataList = _marketDataGateway.GetMarketDataItems(worldID, list);

                if (!marketDataList.Any()) throw new MarketDataNotFoundException();

                // Market Data
                MarketDataPoco marketData = marketDataList.First();
                float marketCost = (float)marketData.averageSold;
                if (marketCost < 1) throw new MarketDataNotFoundException(COST_MISSING_ERROR);
                _logMarketDataInfoSuccess(worldID, itemID);
                ///

                /// Crafting
                var craftingCost = 999999;
                craftingCost = CalculateCraftingCost(marketData);
                LogCraftingCostSuccess(worldID, itemID, craftingCost);
                //

                return craftingCost;
            }
            catch (MarketDataNotFoundException err)
            {
                _logMarketDataNotFoundError(worldID, itemID, err);
                return ERROR_DEFAULT_COST;
            }
        }

        public int CalculateCraftingCost(int worldID, int itemID)
        {
            var lowestCost = int.MaxValue;
            var recipes = _recipeGateway.GetRecipesForItem(itemID);
            foreach (var recipe in recipes)
            {

            }

        }

        public int CalculateCraftingCostForRecipe(int worldID, int recipeID)
        {

        }

        private bool _canMakeRecipe(int recipeID)
        {
            //add functionality here to check for crafting levels per recipe
            return recipeID > 0;
        }

        private static void _logCraftingCostSuccess(int worldID, int itemID, int craftingCost)
        {
            Log.Information(
                $"Returning crafitng cost for itemID: {itemID}, worldID: {worldID}, craftingCost: {craftingCost}",
                itemID,
                worldID,
                craftingCost
            );
        }

        private static void _logMarketDataInfoSuccess(int worldID, int itemID)
        {
            Log.Information(
                $"Found the market data for itemID: {itemID}, worldID: {worldID}.",
                itemID,
                worldID
            );
        }

        private static void _logMarketDataNotFoundError(
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
            "Found the item but no value for averageSale, which used to calculate cost.";
    }
}
