using GilGoblin.pocos;
using GilGoblin.web;
using Serilog;

namespace GilGoblin.crafting
{
    public partial class CraftingCalculator : ICraftingCalculator
    {
        private static RecipeGateway _gateway = new RecipeGateway();
        public IEnumerable<IngredientQty> breakdownRecipe(int recipeID)
        {
            var ingredientList = new List<IngredientQty>();
            var recipe = _gateway.GetRecipe(recipeID);

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
        public int calculateCraftingCost(int itemID, int worldID)
        {
            return random.Next(100,400);
        }
    }
}