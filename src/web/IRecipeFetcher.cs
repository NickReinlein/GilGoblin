using GilGoblin.pocos;

namespace GilGoblin.web
{
    public interface IRecipeGateway
    {
        public RecipeFullPoco GetRecipe(int recipeID);
        public RecipeFullPoco FetchRecipe(int recipeID);
    }
}