using GilGoblin.web;
using Serilog;

namespace GilGoblin.crafting
{
    public partial class CraftingCalculator : ICraftingCalculator
    {    
        private static RecipeGateway _recipeGateway = new RecipeGateway();
        private static MarketDataGateway _marketDataGateway = new MarketDataGateway();

        private const int ERROR_COST = -1;

        public IEnumerable<IngredientQty> breakdownRecipe(int recipeID)
        {
            var ingredientList = new List<IngredientQty>();
            var recipe = _recipeGateway.GetRecipe(recipeID);

            if (recipe is not null ){            
            foreach (var ingredient in recipe.ingredients)
            {
                // if no recipe, can we look it up with the ingredient's ID?
                if (canMakeRecipe(ingredient.recipeID))
                    ingredientList.AddRange(breakdownRecipe(ingredient.recipeID));
            }
            }
            
            return ingredientList;
        }

        private bool canMakeRecipe(int recipeID)
        {
            //add functionality here to check for crafting levels per recipe
            return recipeID is not 0;
        }

        private static Random random = new Random();         //temporary
        public int calculateCraftingCost(int worldID, int itemID)
        {
            try{
                var list = new List<int> { itemID };
                var marketData = _marketDataGateway.GetMarketDataItems(worldID, list)?.First();

                 if (marketData is null) 
                     throw new GilGoblin.web.MarketDataNotFoundException();
                // else if (marketData.averageSale is null) 
                //     throw new MarketDataNotFoundException2("Found the item but a null value for the cost.");
                
                return (int) MathF.Floor((float)marketData.averageSale);
            }
            catch(FileNotFoundException err) {
                Log.Error($"Failed to find market data for itemID: {itemID}, worldID: {worldID}. Error Message: {err}", itemID, worldID, err.Message);
                return ERROR_COST;
            }
        }
    }
}