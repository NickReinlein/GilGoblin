using GilGoblin.pocos;

namespace GilGoblin.web
{
    public interface IRecipeGateway
    {
        public RecipePoco GetRecipe(int recipeID);
        public RecipePoco FetchRecipe(int recipeID);
        public IEnumerable<RecipePoco> GetRecipesForItem(int itemID);
    }
}
