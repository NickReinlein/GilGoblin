using GilGoblin.Pocos;
using Newtonsoft.Json;

namespace GilGoblin.Database;

public class RecipeGateway : IRecipeGateway
{
    public RecipePoco GetRecipe(int recipeID) => new();

    public IEnumerable<RecipePoco> GetRecipesForItem(int itemID) => Array.Empty<RecipePoco>();

    public IEnumerable<RecipePoco> GetRecipes(IEnumerable<int> recipeIDs)
    {
        var recipes = new List<RecipePoco>();
        foreach (var recipe in recipeIDs)
        {
            recipes.Add(GetRecipe(recipe));
        }
        return recipes;
    }

    public IEnumerable<RecipePoco> GetAllRecipes()
    {
        return new List<RecipePoco>();
    }
}
