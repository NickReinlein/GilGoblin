using GilGoblin.pocos;

namespace GilGoblin.crafting
{
    public interface ICraftingCalculator
    {
        public IEnumerable<IngredientPoco> BreakdownRecipe(int recipeID);
        public IEnumerable<IngredientPoco> BreakdownItem(int itemID);
        public int CalculateCraftingCostForRecipe(int worldID, int recipeID);
        public int CalculateCraftingCostForItem(int worldID, int itemID);
        public int CalculateCraftingCostForIngredients(int worldID,
            IEnumerable<CraftIngredient> craftIngredients);
    }
}
