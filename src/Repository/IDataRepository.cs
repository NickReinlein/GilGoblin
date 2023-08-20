using System.Collections.Generic;
using GilGoblin.Cache;

namespace GilGoblin.Repository;

public interface IDataRepository<T> : IRepositoryCache
    where T : class
{
    T? Get(int id);
    IEnumerable<T> GetMultiple(IEnumerable<int> ids);
    IEnumerable<T> GetAll();
}
