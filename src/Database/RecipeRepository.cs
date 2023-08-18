using System.Collections.Generic;
using System.Linq;
using GilGoblin.Cache;
using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Database;

public class RecipeRepository : IRecipeRepository
{
    private readonly GilGoblinDbContext _dbContext;
    private readonly IRecipeCache _cache;

    public RecipeRepository(GilGoblinDbContext recipes, IRecipeCache cache)
    {
        _dbContext = recipes;
        _cache = cache;
    }

    public RecipePoco? Get(int recipeID)
    {
        var cached = _cache.Get(recipeID);
        if (cached is not null)
            return cached;

        var item = _dbContext?.Recipe?.FirstOrDefault(i => i.ID == recipeID);
        if (item is not null)
            _cache.Add(item.ID, item);

        return item;
    }

    public IEnumerable<RecipePoco> GetRecipesForItem(int itemID) =>
        _dbContext.Recipe.Where(r => r.TargetItemID == itemID);

    public IEnumerable<RecipePoco?> GetMultiple(IEnumerable<int> recipeIDs) =>
        _dbContext.Recipe.Where(r => recipeIDs.Any(a => a == r.ID));

    public IEnumerable<RecipePoco> GetAll() => _dbContext.Recipe;
}
