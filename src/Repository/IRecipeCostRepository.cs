using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Cache;
using GilGoblin.Pocos;

namespace GilGoblin.Repository;

public interface IRecipeCostRepository : IRepositoryCache
{
    Task<RecipeCostPoco?> Get(int worldID, int recipeID);
    IEnumerable<RecipeCostPoco> GetMultiple(int worldID, IEnumerable<int> recipeIDs);
    IEnumerable<RecipeCostPoco> GetAll(int worldID);
    Task Add(RecipeCostPoco entity);
}
