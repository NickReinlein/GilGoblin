using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Api.Pocos;

namespace GilGoblin.Api.Crafting;

public interface ICraftingCalculator
{
    Task<int> CalculateCraftingCostForRecipe(int worldId, int recipeId);
    Task<(int, int)> CalculateCraftingCostForItem(int worldId, int itemId);

    Task<int> CalculateCraftingCostForIngredients(
        int worldId,
        IEnumerable<CraftIngredientPoco> craftIngredients
    );
}