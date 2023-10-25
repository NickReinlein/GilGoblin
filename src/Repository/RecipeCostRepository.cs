using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Cache;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Repository;

public class RecipeCostRepository : IRecipeCostRepository
{
    private readonly GilGoblinDbContext _dbContext;
    private static IRecipeCostCache _cache;

    public RecipeCostRepository(GilGoblinDbContext dbContext, IRecipeCostCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<RecipeCostPoco?> GetAsync(int worldId, int recipeId)
    {
        var cached = _cache.Get((worldId, recipeId));
        if (cached is not null)
            return cached;

        var recipeCost = _dbContext.RecipeCost.FirstOrDefault(
            p => p.WorldId == worldId && p.RecipeId == recipeId
        );
        if (recipeCost is not null)
            await Add(recipeCost);

        return recipeCost;
    }

    public IEnumerable<RecipeCostPoco> GetMultiple(int worldId, IEnumerable<int> recipeIds) =>
        _dbContext.RecipeCost.Where(p => p.WorldId == worldId && recipeIds.Any(i => i == p.RecipeId));

    public IEnumerable<RecipeCostPoco> GetAll(int worldId) =>
        _dbContext.RecipeCost.Where(p => p.WorldId == worldId);

    public async Task Add(RecipeCostPoco entity)
    {
        (int, int) key = (entity.WorldId, entity.RecipeId);
        if (_cache.Get(key) is not null)
            return;

        _cache.Add(key, entity);

        if (
            _dbContext.RecipeCost.Any(
                i => i.WorldId == entity.WorldId && i.RecipeId == entity.RecipeId
            )
        )
            return;

        _dbContext.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public Task FillCache()
    {
        var costs = _dbContext?.RecipeCost?.ToList();
        costs?.ForEach(cost => _cache.Add((cost.WorldId, cost.RecipeId), cost));
        return Task.CompletedTask;
    }
}