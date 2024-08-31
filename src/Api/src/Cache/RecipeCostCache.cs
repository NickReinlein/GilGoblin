// using GilGoblin.Database.Pocos;
//
// namespace GilGoblin.Api.Cache;
//
// public interface IRecipeCostCache : IDataCache<(int, int), RecipeCostPoco>
// {
//     void Delete(int worldId, int recipeId);
// }
//
// public class RecipeCostCache : DataCache<(int, int), RecipeCostPoco>, IRecipeCostCache
// {
//     public void Delete(int worldId, int recipeId)
//     {
//         Cache.TryRemove((worldId, recipeId), out _);
//     }
// }