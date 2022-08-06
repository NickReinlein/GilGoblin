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
            foreach (var ingredient in recipe.ingredients){
                if (ingredient.recipeID is not 0) 
                    ingredientList.AddRange(breakdownRecipe(ingredient.recipeID));
            }
            return ingredientList;
        }

        public int calculateCraftingCost(int itemID, int worldID, bool hq = false)
        {
            return hq ? 2 : 1;
        }
    }
}