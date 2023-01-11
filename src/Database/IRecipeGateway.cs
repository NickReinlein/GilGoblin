using GilGoblin.Pocos;

namespace GilGoblin.Database;

public interface IRecipeGateway
{
    public RecipePoco GetRecipe(int recipeID);
    public IEnumerable<RecipePoco> GetRecipes(IEnumerable<int> recipeIDs);
    public IEnumerable<RecipePoco> GetAllRecipes();
    public IEnumerable<RecipePoco> GetRecipesForItem(int itemID);
}
