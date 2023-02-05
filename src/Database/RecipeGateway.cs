using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Database;

public class RecipeGateway : IRecipeRepository
{
    public async Task<RecipePoco?> Get(int recipeID) => new();

    public async Task<IEnumerable<RecipePoco?>> GetRecipesForItem(int itemID) =>
        Array.Empty<RecipePoco>();

    public async Task<IEnumerable<RecipePoco?>> GetMultiple(IEnumerable<int> recipeIDs)
    {
        var recipes = new List<RecipePoco>();
        foreach (var recipe in recipeIDs)
        {
            var result = await Get(recipe);
            if (result is null)
                continue;
            recipes.Add(result);
        }
        return recipes;
    }

    public async Task<IEnumerable<RecipePoco>> GetAll()
    {
        return new List<RecipePoco>();
    }
}
