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

public interface IRecipeProfitRepository : IRepositoryCache
{
    Task<RecipeProfitPoco?> GetAsync(int worldId, int recipeId, bool isHq = false);
    Task<List<RecipeProfitPoco>> GetMultipleAsync(int worldId, IEnumerable<int> recipeIds);
    Task<List<RecipeProfitPoco>> GetAllAsync(int worldId);
}

public class RecipeProfitRepository(IServiceProvider serviceProvider, ICalculatedMetricCache<RecipeProfitPoco> cache)
    : IRecipeProfitRepository
{
    public async Task<RecipeProfitPoco?> GetAsync(int worldId, int recipeId, bool isHq = false)
    {
        var key = new TripleKey(worldId, recipeId, isHq);
        var cached = cache.Get(key);
        if (cached is not null)
        {
            if (DataIsFresh(cached.LastUpdated))
                return cached;
            cache.Delete(key);
        }

        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var recipeProfit = dbContext.RecipeProfit.FirstOrDefault(p =>
            p.WorldId == worldId &&
            p.RecipeId == recipeId &&
            p.IsHq == isHq);
        if (recipeProfit is null)
            return null;

        if (!DataIsFresh(recipeProfit.LastUpdated))
            cache.Delete(key);

        return recipeProfit;
    }

    public async Task<List<RecipeProfitPoco>> GetMultipleAsync(int worldId, IEnumerable<int> recipeIds)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        return await dbContext.RecipeProfit
            .Where(p =>
                p.WorldId == worldId &&
                recipeIds.Any(i => i == p.RecipeId))
            .ToListAsync();
    }

    public async Task<List<RecipeProfitPoco>> GetAllAsync(int worldId)
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        return await dbContext.RecipeProfit
            .Where(p => p.WorldId == worldId)
            .ToListAsync();
    }

    public async Task FillCache()
    {
        await using var scope = serviceProvider.CreateAsyncScope();
        await using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        await dbContext.RecipeProfit
            .ForEachAsync(cost =>
                cache.Add(new TripleKey(cost.WorldId, cost.RecipeId, cost.IsHq), cost));
    }

    private static bool DataIsFresh(DateTimeOffset timestamp) =>
        timestamp >= DateTime.UtcNow.AddHours(-GetDataFreshnessInHours());

    private static int GetDataFreshnessInHours() => 48;
}