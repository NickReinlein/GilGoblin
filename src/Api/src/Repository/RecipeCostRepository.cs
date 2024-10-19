using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.DependencyInjection;

namespace GilGoblin.Api.Repository;

public interface IRecipeCostRepository : IRepositoryCache
{
    Task<RecipeCostPoco?> GetAsync(int worldId, int recipeId, bool isHq);
    IEnumerable<RecipeCostPoco> GetMultiple(int worldId, IEnumerable<int> recipeIds);
    IEnumerable<RecipeCostPoco> GetAll(int worldId);
}


public class RecipeCostRepository(IServiceProvider serviceProvider, IRecipeCostCache cache) : IRecipeCostRepository
{

    public async Task<RecipeCostPoco?> GetAsync(int worldId, int recipeId, bool isHq)
    {
        var cached = cache.Get((worldId, recipeId));
        if (cached is not null)
        {
            if (DataIsFresh(cached.LastUpdated))
                return cached;
            cache.Delete(worldId, recipeId);
        }

        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var recipeCost = dbContext.RecipeCost.FirstOrDefault(p =>
            p.WorldId == worldId &&
            p.RecipeId == recipeId);
        if (recipeCost is null)
            return null;

        await AddAsync(recipeCost);
        return recipeCost;
    }

    private static bool DataIsFresh(DateTimeOffset timestamp) =>
        timestamp >= DateTime.UtcNow.AddHours(-GetDataFreshnessInHours());

    public IEnumerable<RecipeCostPoco> GetMultiple(int worldId, IEnumerable<int> recipeIds) =>
        _dbContext.RecipeCost.Where(p => p.WorldId == worldId && recipeIds.Any(i => i == p.RecipeId));

    public IEnumerable<RecipeCostPoco> GetAll(int worldId) =>
        _dbContext.RecipeCost.Where(p => p.WorldId == worldId);

    public async Task AddAsync(RecipeCostPoco entity)
    {
        (int, int) key = (entity.WorldId, entity.RecipeId);
        var cached = _cache.Get(key);
        if (cached is not null)
        {
            if (DataIsFresh(cached.Updated))
                return;
            _cache.Delete(entity.WorldId, entity.RecipeId);
        }

        _cache.Add(key, entity);

        var existing = await _dbContext.FindAsync<RecipeCostPoco>(entity.RecipeId, entity.WorldId);
        if (existing is null)
        {
            _dbContext.RecipeCost.Add(entity);
        }
        else
        {
            _dbContext.Entry(existing).CurrentValues.SetValues(entity);
        }

        await _dbContext.SaveChangesAsync();
    }

    private static int GetDataFreshnessInHours() => 48;

    public Task FillCache()
    {
        var costs = _dbContext.RecipeCost.ToList();
        costs?.ForEach(cost => _cache.Add((cost.WorldId, cost.RecipeId), cost));
        return Task.CompletedTask;
    }
}