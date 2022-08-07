using GilGoblin.pocos;
using GilGoblin.web;
using Serilog;

namespace GilGoblin.crafting
{
    public partial class CraftingCalculator : ICraftingCalculator
    {
        private static RecipeFetcher _fetcher = new RecipeFetcher();
        public IEnumerable<IngredientQty> breakdownRecipe(int recipeID)
        {
            // get recipe
            // foreach on ingredient
            // if recipeExists && CanMake
            // breakdown each ingredient by calling self
            // return
            var ingredientList = new List<IngredientQty>();
            var recipe = _fetcher.GetRecipeByID(recipeID);
            if (recipe is null) return Array.Empty<IngredientQty>();
            foreach (var ingredient in recipe.ingredients)
            {
                if (canMakeRecipe(ingredient.recipeID))
                    ingredientList.AddRange(breakdownRecipe(ingredient.recipeID));
                // if no recipe, can we look it up with the ingredient's ID?
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