// using GilGoblin.Database.Pocos;
//
// namespace GilGoblin.Api.Cache;
//
// public interface IRecipeProfitCache : IDataCache<(int, int), RecipeProfitPoco>
// {
//     void Delete(int worldId, int recipeId);
// }
//
// public class RecipeProfitCache : DataCache<(int, int), RecipeProfitPoco>, IRecipeProfitCache
// {
//     public void Delete(int worldId, int recipeId)
//     {
//         Cache.TryRemove((worldId, recipeId), out _);
//     }
// }