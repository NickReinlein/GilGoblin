using GilGoblin.web;
using Serilog;

namespace GilGoblin.crafting
{
    public partial class CraftingCalculator : ICraftingCalculator
    {
        private static IRecipeGateway _recipeGateway = new RecipeGateway();
        private static IMarketDataGateway _marketDataGateway = new MarketDataGateway();

        public static int ERROR_DEFAULT_COST { get; } = -1;

        public IEnumerable<IngredientQty> BreakdownRecipe(int recipeID)
        {
            var ingredientList = new List<IngredientQty>();
            var recipe = _recipeGateway.GetRecipe(recipeID);

            if (recipe is not null)
            {
                foreach (var ingredient in recipe.ingredients)
                {
                    // if no recipe, can we look it up with the ingredient's ID?
                    if (canMakeRecipe(ingredient.recipeID))
                        ingredientList.AddRange(BreakdownRecipe(ingredient.recipeID));
                }
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
                var marketData = _marketDataGateway.GetMarketDataItems(worldID, list)?.First();

                if (marketData is null)
                    throw new MarketDataNotFoundException();
                else if (marketData.averageSale is 0)
                    throw new MarketDataNotFoundException(
                        "Found the item but a null value for the cost."
                    );

                return (int)MathF.Floor((float)marketData.averageSale);
            }
            catch (FileNotFoundException err)
            {
                Log.Error(
                    $"Failed to find market data for itemID: {itemID}, worldID: {worldID}. Error Message: {err}",
                    itemID,
                    worldID,
                    err.Message
                );
                return ERROR_DEFAULT_COST;
            }
        }
    }
}
