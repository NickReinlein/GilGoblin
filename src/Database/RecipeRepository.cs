using System.Collections.Generic;
using System.Linq;
using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Database;

public class RecipeRepository : IRecipeRepository
{
    private readonly GilGoblinDbContext _dbContext;

    public RecipeRepository(GilGoblinDbContext recipes)
    {
        _dbContext = recipes;
    }

    public RecipePoco? Get(int recipeID) => _dbContext.Recipe.FirstOrDefault(r => r.ID == recipeID);

    public IEnumerable<RecipePoco> GetRecipesForItem(int itemID) =>
        _dbContext.Recipe.Where(r => r.TargetItemID == itemID);

    public IEnumerable<RecipePoco?> GetMultiple(IEnumerable<int> recipeIDs) =>
        _dbContext.Recipe.Where(r => recipeIDs.Any(a => a == r.ID));

    public IEnumerable<RecipePoco> GetAll() => _dbContext.Recipe;
}
