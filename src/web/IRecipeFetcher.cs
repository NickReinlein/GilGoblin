using GilGoblin.pocos;

namespace GilGoblin.web
{
    public interface IRecipeFetcher
    {
        public RecipeFullPoco GetRecipeByID(int recipeID);
    }
}