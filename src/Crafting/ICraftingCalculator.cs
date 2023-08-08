using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Pocos;

namespace GilGoblin.Crafting;

public interface ICraftingCalculator
{
    Task<int> CalculateCraftingCostForRecipe(int worldID, int recipeID);
    Task<(int, float)> CalculateCraftingCostForItem(int worldID, int itemID);
    Task<int> CalculateCraftingCostForIngredients(
        int worldID,
        IEnumerable<CraftIngredientPoco> craftIngredients
    );
}
