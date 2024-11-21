using System.Collections.Generic;
using GilGoblin.Api.Cache;

namespace GilGoblin.Api.Repository;

public interface IDataRepository<T> : IRepositoryCache
{
    T? Get(int itemId);
    List<T> GetMultiple(IEnumerable<int> ids);
    List<T> GetAll();
}
