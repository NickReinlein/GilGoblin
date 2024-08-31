// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading.Tasks;
// using GilGoblin.Api.Cache;
// using GilGoblin.Database;
// using GilGoblin.Database.Pocos;
//
// namespace GilGoblin.Api.Repository;
//
// public class RecipeProfitRepository : IRecipeProfitRepository
// {
//     private readonly GilGoblinDbContext _dbContext;
//     private static IRecipeProfitCache _cache;
//
//     public RecipeProfitRepository(GilGoblinDbContext dbContext, IRecipeProfitCache cache)
//     {
//         _dbContext = dbContext;
//         _cache = cache;
//     }
//
//     public async Task<RecipeProfitPoco?> GetAsync(int worldId, int recipeId)
//     {
//         var cached = _cache.Get((worldId, recipeId));
//         if (cached is not null)
//         {
//             if (DataIsFresh(cached.Updated))
//                 return cached;
//             _cache.Delete(worldId, recipeId);
//         }
//
//         var recipeProfit = _dbContext.RecipeProfit.FirstOrDefault(p =>
//             p.WorldId == worldId &&
//             p.RecipeId == recipeId);
//         if (recipeProfit is null)
//             return null;
//
//         if (DataIsFresh(recipeProfit.Updated))
//             await AddAsync(recipeProfit);
//         else
//             _cache.Delete(worldId, recipeId);
//
//         return recipeProfit;
//     }
//
//     public IEnumerable<RecipeProfitPoco> GetMultiple(int worldId, IEnumerable<int> recipeIds) =>
//         _dbContext.RecipeProfit.Where(p => p.WorldId == worldId && recipeIds.Any(i => i == p.RecipeId));
//
//     public IEnumerable<RecipeProfitPoco> GetAll(int worldId) =>
//         _dbContext.RecipeProfit.Where(p => p.WorldId == worldId);
//
//     public async Task AddAsync(RecipeProfitPoco? entity)
//     {
//         if (entity is null)
//             return;
//
//         (int, int) key = (entity.WorldId, entity.RecipeId);
//         var cached = _cache.Get(key);
//         if (cached is not null)
//         {
//             if (DataIsFresh(cached.Updated))
//                 return;
//             _cache.Delete(entity.WorldId, entity.RecipeId);
//         }
//
//         _cache.Add(key, entity);
//
//         var existing = await _dbContext.FindAsync<RecipeProfitPoco>(entity.RecipeId, entity.WorldId);
//         if (existing is null)
//         {
//             _dbContext.RecipeProfit.Add(entity);
//         }
//         else
//         {
//             _dbContext.Entry(existing).CurrentValues.SetValues(entity);
//         }
//
//         await _dbContext.SaveChangesAsync();
//     }
//
//     public Task FillCache()
//     {
//         var costs = _dbContext.RecipeProfit.ToList();
//         costs?.ForEach(cost => _cache.Add((cost.WorldId, cost.RecipeId), cost));
//         return Task.CompletedTask;
//     }
//
//     private static bool DataIsFresh(DateTimeOffset timestamp) =>
//         timestamp >= DateTime.UtcNow.AddHours(-GetDataFreshnessInHours());
//
//     private static int GetDataFreshnessInHours() => 48;
// }