using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Repository;

public interface IRecipeCostRepository : IRepositoryCache
{
    Task<RecipeCostPoco?> GetAsync(int worldId, int recipeId);
    IEnumerable<RecipeCostPoco> GetMultiple(int worldId, IEnumerable<int> recipeIds);
    IEnumerable<RecipeCostPoco> GetAll(int worldId);
    Task AddAsync(RecipeCostPoco entity);
}
