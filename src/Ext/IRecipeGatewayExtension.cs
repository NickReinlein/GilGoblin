using GilGoblin.Pocos;

namespace GilGoblin.Ext
{
    public interface IRecipeGatewayExtension
    {
        public IEnumerable<IngredientPoco> BreakdownRecipe(int recipeID);
        public IEnumerable<IngredientPoco> BreakdownItem(int itemID);
    }
}
