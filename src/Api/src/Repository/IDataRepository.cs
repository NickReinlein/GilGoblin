using System.Collections.Generic;
using GilGoblin.Api.Cache;

namespace GilGoblin.Api.Repository;

public interface IDataRepository<out T> : IRepositoryCache
{
    T? Get(int itemId);
    IEnumerable<T> GetMultiple(IEnumerable<int> itemIds);
    IEnumerable<T> GetAll();
}
