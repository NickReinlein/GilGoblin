using System.Collections.Generic;
using GilGoblin.Api.Cache;

namespace GilGoblin.Api.Repository;

public interface IPriceRepository<T> : IRepositoryCache
    where T : class
{
    T? Get(int worldId, int id, bool isHq);
    IEnumerable<T> GetMultiple(int worldId, IEnumerable<int> ids, bool isHq);
    IEnumerable<T> GetAll(int worldId);
}
