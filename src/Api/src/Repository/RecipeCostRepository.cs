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
    Task<RecipeCostPoco?> GetAsync(int worldId, int recipeId, bool isHq = false);
    Task<List<RecipeCostPoco>> GetMultipleAsync(int worldId, IEnumerable<int> ids);
    Task<List<RecipeCostPoco>> GetAllAsync(int worldId);
}

public class RecipeCostRepository(IServiceProvider serviceProvider, ICalculatedMetricCache<RecipeCostPoco> cache)
    : IRecipeCostRepository
{
    public async Task<RecipeCostPoco?> GetAsync(int worldId, int recipeId, bool isHq = false)
    {
        var cached = cache.Get(new TripleKey(worldId, recipeId, isHq));
        if (cached is not null)
        {
            if (IsDataFresh(cached.LastUpdated))
                return cached;
            cache.Delete(new TripleKey(worldId, recipeId, isHq));
        }

        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        return dbContext.RecipeCost.FirstOrDefault(p =>
            p.RecipeId == recipeId &&
            p.WorldId == worldId &&
            p.IsHq == isHq);
    }

    public async Task<List<RecipeCostPoco>> GetMultipleAsync(int worldId, IEnumerable<int> ids)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var recipeCostPocos = dbContext.RecipeCost
            .Where(p =>
                p.WorldId == worldId &&
                ids.Contains(p.RecipeId))
            .ToList();
        return recipeCostPocos;
    }

    public async Task<List<RecipeCostPoco>> GetAllAsync(int worldId)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var recipeCostPocos = dbContext.RecipeCost
            .Where(p => p.WorldId == worldId)
            .ToList();
        return recipeCostPocos;
    }

    public async Task FillCache()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        await dbContext.RecipeCost
            .ForEachAsync(cost =>
                cache.Add(new TripleKey(cost.WorldId, cost.RecipeId, cost.IsHq), cost));
    }

    private static int DataFreshnessInHours => 48;

    private static bool IsDataFresh(DateTimeOffset timestamp) =>
        timestamp >= DateTime.UtcNow.AddHours(-DataFreshnessInHours);
}