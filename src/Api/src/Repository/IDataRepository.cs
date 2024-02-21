using System.Collections.Generic;
using GilGoblin.Api.Cache;

namespace GilGoblin.Api.Repository;

public interface IDataRepository<out T> : IRepositoryCache
{
    T? Get(int recipeId);
    IEnumerable<T> GetMultiple(IEnumerable<int> recipeIds);
    IEnumerable<T> GetAll();
}
