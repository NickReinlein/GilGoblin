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
            var recipes = _recipeGateway.GetRecipesForItem(itemID);
            int recipeCount = recipes.Count();
            var craftingCost = getLowestCraftingCost(worldID, recipes);

            if (recipeCount > 0 && craftingCost == ERROR_DEFAULT_COST)
                LogErrorCraftingCostForItem(worldID, itemID, recipeCount);


            Log.Information("Successfully calculated crafting cost of {LowestCost} for item {ItemID} world {WorldID} with {RecipeCount} craftable recipes",
                    craftingCost, itemID, worldID, recipeCount);
            return craftingCost;
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
                int averageSold = (int)craft.MarketData.AverageSold;
                int craftingCost = CalculateCraftingCostForItem(worldID, craft.ItemID);
                var minCost = Math.Min(averageSold, craftingCost);

                Log.Debug("Calculated cost {MinCost} for {ItemID} in world {WorldID}, based on sell price {Sold} and crafting cost {CraftCost}", 
                    minCost, craft.ItemID, worldID, averageSold, craftingCost);
                totalCraftingCost += craft.Quantity * minCost;
            }

            return totalCraftingCost;
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


        public static List<CraftIngredient> _makeCraftIngredients(IEnumerable<IngredientPoco> ingredients, IEnumerable<MarketDataPoco> marketData)
        {
            List<CraftIngredient> crafts = new List<CraftIngredient>();
            foreach (var ingredient in ingredients)
            {
                var market = marketData.Single(e => e.ItemID == ingredient.ItemID);
                if (market is null) throw new MarketDataNotFoundException();

                crafts.Add(new CraftIngredient(ingredient, market));
            }
            return crafts;
        }

        private IEnumerable<MarketDataPoco> _getIngredientMarketData(int worldID, int itemID, IEnumerable<IngredientPoco> ingredients)
        {
            var itemIDList = ingredients.Select(e => e.ItemID).ToList();
            itemIDList.Add(itemID);
            itemIDList.Sort();
            var marketData = _marketDataGateway.GetMarketDataItems(worldID, itemIDList);
            if (!marketData.Any()) throw new MarketDataNotFoundException();
            return marketData;
        }

        private int getLowestCraftingCost(int worldID, IEnumerable<RecipePoco> recipes)
        {
            var lowestCost = ERROR_DEFAULT_COST;
            foreach (var recipe in recipes)
            {
                var recipeCost = CalculateCraftingCostForRecipe(worldID, recipe.recipeID);
                lowestCost = Math.Min(recipeCost, lowestCost);
            }
            return lowestCost;
        }
    
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
