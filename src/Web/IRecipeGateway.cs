using GilGoblin.Pocos;

namespace GilGoblin.Web;

public interface IRecipeRepository
{
    public RecipePoco GetRecipe(int recipeID);
    public RecipePoco FetchRecipe(int recipeID);
    public IEnumerable<RecipePoco> GetRecipesForItem(int itemID);
}
