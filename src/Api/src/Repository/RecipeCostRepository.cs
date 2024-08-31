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
// public class RecipeCostRepository : IRecipeCostRepository
// {
//     private readonly GilGoblinDbContext _dbContext;
//     private static IRecipeCostCache _cache;
//
//     public RecipeCostRepository(GilGoblinDbContext dbContext, IRecipeCostCache cache)
//     {
//         _dbContext = dbContext;
//         _cache = cache;
//     }
//
//     public async Task<RecipeCostPoco?> GetAsync(int worldId, int recipeId)
//     {
//         var cached = _cache.Get((worldId, recipeId));
//         if (cached is not null)
//         {
//             if (DataIsFresh(cached.Updated))
//                 return cached;
//             _cache.Delete(worldId, recipeId);
//         }
//
//         var recipeCost = _dbContext.RecipeCost.FirstOrDefault(p =>
//             p.WorldId == worldId &&
//             p.RecipeId == recipeId);
//         if (recipeCost is null)
//             return null;
//
//         if (DataIsFresh(recipeCost.Updated))
//             await AddAsync(recipeCost);
//         else
//             _cache.Delete(worldId, recipeId);
//
//         return recipeCost;
//     }
//
//     private static bool DataIsFresh(DateTimeOffset timestamp) =>
//         timestamp >= DateTime.UtcNow.AddHours(-GetDataFreshnessInHours());
//
//     public IEnumerable<RecipeCostPoco> GetMultiple(int worldId, IEnumerable<int> recipeIds) =>
//         _dbContext.RecipeCost.Where(p => p.WorldId == worldId && recipeIds.Any(i => i == p.RecipeId));
//
//     public IEnumerable<RecipeCostPoco> GetAll(int worldId) =>
//         _dbContext.RecipeCost.Where(p => p.WorldId == worldId);
//
//     public async Task AddAsync(RecipeCostPoco entity)
//     {
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
//         var existing = await _dbContext.FindAsync<RecipeCostPoco>(entity.RecipeId, entity.WorldId);
//         if (existing is null)
//         {
//             _dbContext.RecipeCost.Add(entity);
//         }
//         else
//         {
//             _dbContext.Entry(existing).CurrentValues.SetValues(entity);
//         }
//
//         await _dbContext.SaveChangesAsync();
//     }
//
//     private static int GetDataFreshnessInHours() => 48;
//
//     public Task FillCache()
//     {
//         var costs = _dbContext.RecipeCost.ToList();
//         costs?.ForEach(cost => _cache.Add((cost.WorldId, cost.RecipeId), cost));
//         return Task.CompletedTask;
//     }
// }