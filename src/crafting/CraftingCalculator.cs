using GilGoblin.pocos;
using GilGoblin.web;
using Serilog;

namespace GilGoblin.crafting
{
    public partial class CraftingCalculator : ICraftingCalculator
    {
        private static RecipeGateway _recipeGateway = new RecipeGateway();
        private static MarketDataGateway _marketDataGateway = new MarketDataGateway();
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
            var list = new List<int> { itemID };
            int cost = (int)_marketDataGateway.GetMarketDataItems(worldID, list)
                                              .FirstOrDefault().averageListingPrice;
            return 0;
        }
    }
}