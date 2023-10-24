using System.Collections.Generic;

namespace GilGoblin.Cache;

public interface IDataCache<T, U>
    where U : class
{
    void Add(T key, U item);

    U? Get(T key);
    IEnumerable<U> GetMultiple(IEnumerable<T> keys);
    IEnumerable<U> GetAll();
    IEnumerable<T> GetKeys();
}
