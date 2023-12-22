using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Repository;

public class RecipeProfitRepository : IRecipeProfitRepository
{
    private readonly GilGoblinDbContext _dbContext;
    private static IRecipeProfitCache _cache;

    public RecipeProfitRepository(GilGoblinDbContext dbContext, IRecipeProfitCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<RecipeProfitPoco> GetAsync(int worldId, int recipeId)
    {
        var cached = _cache.Get((worldId, recipeId));
        if (cached is not null)
            return cached;

        var recipeProfit = _dbContext.RecipeProfit
            .FirstOrDefault(p => p.WorldId == worldId && p.RecipeId == recipeId);
        if (recipeProfit is not null)
            await Add(recipeProfit);

        return recipeProfit;
    }

    public IEnumerable<RecipeProfitPoco> GetMultiple(int worldId, IEnumerable<int> recipeIds) =>
        _dbContext.RecipeProfit.Where(p => p.WorldId == worldId && recipeIds.Any(i => i == p.RecipeId));

    public IEnumerable<RecipeProfitPoco> GetAll(int worldId) =>
        _dbContext.RecipeProfit.Where(p => p.WorldId == worldId);

    public async Task Add(RecipeProfitPoco entity)
    {
        (int, int) key = (entity.WorldId, entity.RecipeId);
        if (_cache.Get(key) is not null)
            return;

        _cache.Add(key, entity);

        if (_dbContext.RecipeProfit
            .Any(i =>
                i.WorldId == entity.WorldId &&
                i.RecipeId == entity.RecipeId))
            return;

        _dbContext.Add(entity);
        await _dbContext.SaveChangesAsync();
    }

    public Task FillCache()
    {
        var costs = _dbContext?.RecipeProfit?.ToList();
        costs?.ForEach(cost => _cache.Add((cost.WorldId, cost.RecipeId), cost));
        return Task.CompletedTask;
    }
}