using System.Collections.Generic;
using GilGoblin.Api.Cache;

namespace GilGoblin.Api.Repository;

public interface IDataRepository<T> : IRepositoryCache
    where T : class
{
    T? Get(int recipeId);
    IEnumerable<T> GetMultiple(IEnumerable<int> recipeIds);
    IEnumerable<T> GetAll();
}
