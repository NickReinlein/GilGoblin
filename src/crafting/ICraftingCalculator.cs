using GilGoblin.pocos;

namespace GilGoblin.crafting
{
    public interface ICraftingCalculator
    {
        public int GetBestCostForItem(int worldID, int itemID);
        public IEnumerable<IngredientPoco> BreakdownRecipe(int recipeID);
    }
}
