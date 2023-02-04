using GilGoblin.Pocos;

namespace GilGoblin.Crafting;

public interface IRecipeGrocer
{
    public Task<IEnumerable<IngredientPoco?>> BreakdownRecipe(int recipeID);
    public Task<IEnumerable<IngredientPoco?>> BreakdownItem(int itemID);
}
