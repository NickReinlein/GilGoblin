using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Repository;

public interface IRecipeProfitRepository : IRepositoryCache
{
    Task<RecipeProfitPoco?> GetAsync(int worldId, int recipeId);
    IEnumerable<RecipeProfitPoco> GetMultiple(int worldId, IEnumerable<int> recipeIds);
    IEnumerable<RecipeProfitPoco> GetAll(int worldId);
    Task AddAsync(RecipeProfitPoco? entity);
}