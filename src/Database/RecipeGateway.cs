using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Database;

public class RecipeGateway : IRecipeRepository
{
    private readonly IRecipeRepository _recipes;

    public RecipeGateway(IRecipeRepository recipes)
    {
        _recipes = recipes;
    }

    public async Task<RecipePoco?> Get(int recipeID) => await _recipes.Get(recipeID);

    public async Task<IEnumerable<RecipePoco?>> GetRecipesForItem(int itemID) =>
        await _recipes.GetRecipesForItem(itemID);

    public async Task<IEnumerable<RecipePoco?>> GetMultiple(IEnumerable<int> recipeIDs) =>
        await _recipes.GetMultiple(recipeIDs);

    public async Task<IEnumerable<RecipePoco>> GetAll() => await _recipes.GetAll();
}
