using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Cache;
using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Database;

public class RecipeCostRepository : IRecipeCostRepository
{
    private readonly GilGoblinDbContext _dbContext;
    private static IRecipeCostCache _cache;

    public RecipeCostRepository(GilGoblinDbContext dbContext, IRecipeCostCache cache)
    {
        _dbContext = dbContext;
        _cache ??= cache;
    }

    public async Task<RecipeCostPoco?> Get(int worldID, int recipeID)
    {
        var cached = _cache.Get((worldID, recipeID));
        if (cached is not null)
            return cached;

        var recipeCost = _dbContext.RecipeCost.FirstOrDefault(
            p => p.WorldID == worldID && p.RecipeID == recipeID
        );
        if (recipeCost is not null)
            await Add(recipeCost);

        return recipeCost;
    }

    public IEnumerable<RecipeCostPoco> GetMultiple(int worldID, IEnumerable<int> ids) =>
        _dbContext.RecipeCost.Where(p => p.WorldID == worldID && ids.Any(i => i == p.RecipeID));

    public IEnumerable<RecipeCostPoco> GetAll(int worldID) =>
        _dbContext.RecipeCost.Where(p => p.WorldID == worldID);

    public async Task FillCache()
    {
        var items = await _dbContext?.RecipeCost?.ToListAsync();
        items.ForEach(cost => _cache.Add((cost.WorldID, cost.RecipeID), cost));
    }

    public async Task Add(RecipeCostPoco entity)
    {
        (int, int) key = (entity.WorldID, entity.RecipeID);
        if (_cache.Get(key) is not null)
            return;

        _cache.Add(key, entity);

        if (
            _dbContext.RecipeCost.Any(
                i => i.WorldID == entity.WorldID && i.RecipeID == entity.RecipeID
            )
        )
            return;

        _dbContext?.Add(entity);
        await _dbContext.SaveChangesAsync();
    }
}
