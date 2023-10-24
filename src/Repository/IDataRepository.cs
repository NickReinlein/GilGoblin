using System.Collections.Generic;
using GilGoblin.Cache;

namespace GilGoblin.Repository;

public interface IDataRepository<T> : IRepositoryCache
    where T : class
{
    T? Get(int recipeId);
    IEnumerable<T> GetMultiple(IEnumerable<int> recipeIds);
    IEnumerable<T> GetAll();
}
