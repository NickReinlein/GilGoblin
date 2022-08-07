using GilGoblin.pocos;

namespace GilGoblin.crafting
{
    public interface ICraftingCalculator {
        public int calculateCraftingCost(int itemID, int worldID);
        public IEnumerable<IngredientQty> breakdownRecipe(int recipeID);
    }
}