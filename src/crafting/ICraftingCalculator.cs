using GilGoblin.pocos;

namespace GilGoblin.crafting
{
    public interface ICraftingCalculator
    {
        public int CalculateCraftingCost(int worldID, int itemID);
        public IEnumerable<IngredientPoco> BreakdownRecipe(int recipeID);
    }
}
