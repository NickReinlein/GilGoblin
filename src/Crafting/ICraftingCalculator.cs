using GilGoblin.Pocos;

namespace GilGoblin.Crafting;

public interface ICraftingCalculator
{
    public int CalculateCraftingCostForRecipe(int worldID, int recipeID);
    public int CalculateCraftingCostForItem(int worldID, int itemID);
    public int CalculateCraftingCostForIngredients(int worldID,
        IEnumerable<CraftIngredient> craftIngredients);
}
