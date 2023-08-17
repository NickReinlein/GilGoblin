using System.Collections.Generic;

namespace GilGoblin.Cache;

public interface IDataCache<T, U>
    where U : class
{
    public void Add(T key, U item);

    public U? Get(T key);

    public IEnumerable<U> GetMultiple(IEnumerable<T> keys);

    public IEnumerable<U> GetAll();

    public IEnumerable<T> GetKeys();
}
