using GilGoblin.pocos;
using GilGoblin.web;
using Serilog;

namespace GilGoblin.crafting
{
    public class CraftingCalculator : ICraftingCalculator
    {
        private readonly IRecipeGateway _recipeGateway;
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

        public static int ERROR_DEFAULT_COST { get; } = int.MaxValue;

        public int CalculateCraftingCostForItem(int worldID, int itemID)
        {
            var lowestCost = ERROR_DEFAULT_COST;
            var recipes = _recipeGateway.GetRecipesForItem(itemID);
            foreach (var recipe in recipes)
            {
                var recipeCost = CalculateCraftingCostForRecipe(worldID, recipe.recipeID);
                lowestCost = Math.Min(recipeCost, lowestCost);
            }

            if (recipes.Count() > 0 && lowestCost == ERROR_DEFAULT_COST)
                LogErrorCraftingCostForItem(worldID, itemID, recipes.Count());
            return lowestCost;
        }

        public int CalculateCraftingCostForRecipe(int worldID, int recipeID)
        {
            try
            {
                var recipe = _recipeGateway.GetRecipe(recipeID);
                var ingredients = BreakdownRecipe(recipeID);
                if (recipe is null || !ingredients.Any()) return ERROR_DEFAULT_COST;

                IEnumerable<MarketDataPoco> ingredientsMarketData = _getIngredientMarketData(worldID, recipe.targetItemID, ingredients);
                IEnumerable<CraftIngredient> craftIngredients = _makeCraftIngredients(ingredients, ingredientsMarketData);

                int craftingCost = CalculateCraftingCostForIngredients(worldID, craftIngredients);

                Log.Information("Successfully calculated crafting cost of {CraftCost} for recipe {RecipeID} world {WorldID} with {IngCount} ingredients",   
                    craftingCost, recipeID, worldID, ingredients.Count());
                return craftingCost;
            }
            catch (MarketDataNotFoundException){
                Log.Error("Failed to find market data while calculating crafting cost for recipe {RecipeID} in world {WorldID} for item {ItemID}", recipeID, worldID);
                return ERROR_DEFAULT_COST;
            }
            catch (Exception e){
                Log.Error("Failed to calculate crafting cost: {e}", e.Message);
             return ERROR_DEFAULT_COST;   
            }
        }

        public int CalculateCraftingCostForIngredients(int worldID, IEnumerable<CraftIngredient> craftIngredients)
        {
            int totalCraftingCost = 0;
            foreach (var craft in craftIngredients)
            {
                int averageSold = (int)craft.MarketData.averageSold;
                int craftingCost = CalculateCraftingCostForItem(worldID, craft.ItemID);
                var minCost = Math.Min(averageSold, craftingCost);

                Log.Debug("Calculated cost {MinCost} for {ItemID} in world {WorldID}, based on sell price {Sold} and crafting cost {CraftCost}", 
                    minCost, craft.ItemID, worldID, averageSold, craftingCost);
                totalCraftingCost += craft.Quantity * minCost;
            }

            return totalCraftingCost;
        }

        private IEnumerable<MarketDataPoco> _getIngredientMarketData(int worldID, int itemID, IEnumerable<IngredientPoco> ingredients)
        {
            var itemIDList = ingredients.Select(e => e.ItemID).ToList();
            var marketData = _marketDataGateway.GetMarketDataItems(worldID, new List<int>() { itemID });
            var ingredientsMarketData = _marketDataGateway.GetMarketDataItems(worldID, itemIDList);
            if (marketData is null || !ingredientsMarketData.Any()) throw new MarketDataNotFoundException();
            return ingredientsMarketData;
        }

        private static List<CraftIngredient> _makeCraftIngredients(IEnumerable<IngredientPoco> ingredients, IEnumerable<MarketDataPoco> marketData)
        {
            List<CraftIngredient> crafts = new List<CraftIngredient>();
            foreach (var ingredient in ingredients)
            {
                var market = marketData.Single(e => e.itemID == ingredient.ItemID);
                if (market is null) throw new MarketDataNotFoundException();

                crafts.Add(new CraftIngredient(ingredient, market));
            }
            return crafts;
        }

        public IEnumerable<IngredientPoco> BreakdownRecipe(int recipeID)
        {
            var ingredientList = new List<IngredientPoco>();

            var recipe = _recipeGateway.GetRecipe(recipeID);
            if (recipe is null) return Array.Empty<IngredientPoco>();

            foreach (var ingredient in recipe.ingredients)
            {
                var breakdownIngredient = BreakdownItem(ingredient.ItemID);
                if (breakdownIngredient.Any())
                    ingredientList.AddRange(breakdownIngredient);
                else
                    ingredientList.Add(ingredient);
            }
            return ingredientList;
        }

        public IEnumerable<IngredientPoco> BreakdownItem(int itemID)
        {
            var ingredientRecipes = _recipeGateway.GetRecipesForItem(itemID);

            foreach (var ingredientRecipe in ingredientRecipes)
            {
                int ingredientRecipeID = ingredientRecipe.recipeID;
                if (_canMakeRecipe(ingredientRecipeID))
                {
                    var recipeIngredients = BreakdownRecipe(ingredientRecipeID);
                    // todo later: each breakdown -> get best price -> choose besingredientst breakdwon                                            
                    // for now we choose the first one
                    if (recipeIngredients.Any()) 
                        return recipeIngredients;
                }
            }
            return Array.Empty<IngredientPoco>();
        }

        // public int GetBestCostForItem(int worldID, int itemID)
        // {
        //     try
        //     {
        //         var list = new List<int> { itemID };
        //         var marketDataList = _marketDataGateway.GetMarketDataItems(worldID, list);

        //         if (!marketDataList.Any()) throw new MarketDataNotFoundException();

        //         // Market Data
        //         MarketDataPoco marketData = marketDataList.First();
        //         float marketCost = (float)marketData.averageSold;
        //         if (marketCost < 1) throw new MarketDataNotFoundException(COST_MISSING_ERROR);
        //         _logMarketDataInfoSuccess(worldID, itemID);
        //         ///

        //         /// Crafting
        //         var craftingCost = ERROR_DEFAULT_COST;
        //         // craftingCost = CalculateCraftingCost(marketData);
        //         if (craftingCost == ERROR_DEFAULT_COST)
        //             _logCraftingCostError(worldID, itemID, craftingCost);
        //         else    
        //             _logCraftingCostSuccess(worldID, itemID, craftingCost);
        //         //

        //         return craftingCost;
        //     }
        //     catch (MarketDataNotFoundException err)
        //     {
        //         _logMarketDataNotFoundError(worldID, itemID, err);
        //         return ERROR_DEFAULT_COST;
        //     }
        // }
        private bool _canMakeRecipe(int recipeID)
        {
            //add functionality here to check for crafting levels per recipe
            return recipeID > 0;
        }

                private void LogErrorCraftingCostForItem(int worldID, int itemID, int recipesCount)
        {
            _log.Error("Failed to calculate crafting cost of: world {worldID}, item {itemID} despite having {count} recipes", worldID, itemID, recipesCount);
        }

        private static void _logCraftingCostSuccess(int worldID, int itemID, int craftingCost)
        {
            Log.Information(
                $"Successfully calculated crafitng cost for itemID: {itemID}, worldID: {worldID}, craftingCost: {craftingCost}",
                itemID,
                worldID,
                craftingCost
            );
        }

        private void _logCraftingCostError(int worldID, int itemID, int craftingCost)
        {
            Log.Error(
                $"Error calculating crafitng cost for itemID: {itemID}, worldID: {worldID}, craftingCost: {craftingCost}",
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
