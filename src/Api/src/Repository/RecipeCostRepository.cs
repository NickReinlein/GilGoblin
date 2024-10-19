using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GilGoblin.Api.Repository;

public interface IRecipeCostRepository : IRepositoryCache
{
    Task<RecipeCostPoco?> GetAsync(int worldId, int recipeId, bool isHq);
    Task<List<RecipeCostPoco>> GetMultiple(int worldId, IEnumerable<int> recipeIds);
    Task<List<RecipeCostPoco>> GetAll(int worldId);
}

public class RecipeCostRepository(IServiceProvider serviceProvider, IRecipeCostCache cache) : IRecipeCostRepository
{
    public async Task<RecipeCostPoco?> GetAsync(int worldId, int recipeId, bool isHq)
    {
        var cached = cache.Get((worldId, recipeId, isHq));
        if (cached is not null)
        {
            if (DataIsFresh(cached.LastUpdated))
                return cached;
            cache.Delete((worldId, recipeId, isHq));
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

    public async Task<List<RecipeCostPoco>> GetMultiple(int worldId, IEnumerable<int> recipeIds)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        return dbContext.RecipeCost
            .Where(p => p.WorldId == worldId && recipeIds.Any(i => i == p.RecipeId))
            .ToList();
    }


    public async Task<List<RecipeCostPoco>> GetAll(int worldId)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        return dbContext.RecipeCost.Where(p => p.WorldId == worldId)
            .ToList();
    }

    public async Task AddAsync(RecipeCostPoco entity)
    {
        (int, int, bool) key = (entity.WorldId, entity.RecipeId, entity.IsHq);
        var cached = cache.Get(key);
        if (cached is not null)
        {
            if (DataIsFresh(cached.LastUpdated))
                return;
            cache.Delete(key);
        }

        cache.Add(key, entity);

        var existing = await GetAsync(key.Item1, key.Item2, key.Item3);

        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        if (existing is null)
            dbContext.RecipeCost.Add(entity);
        else
            dbContext.Entry(existing).CurrentValues.SetValues(entity);

        await dbContext.SaveChangesAsync();
    }

    private static int GetDataFreshnessInHours() => 48;

    public async Task FillCache()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        await dbContext.RecipeCost
            .ForEachAsync(cost =>
                cache.Add((cost.WorldId, cost.RecipeId, cost.IsHq), cost));
    }
}