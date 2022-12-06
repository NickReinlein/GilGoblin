using GilGoblin.Pocos;

namespace GilGoblin.Crafting;

public interface IRecipeGrocer
{
    public IEnumerable<IngredientPoco> BreakdownRecipe(int recipeID);
    public IEnumerable<IngredientPoco> BreakdownItem(int itemID);
}
