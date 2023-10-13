using System.Collections.Generic;

namespace GilGoblin.Cache;

public class DataCache<T, U> : IDataCache<T, U>
    where U : class
{
    protected Dictionary<T, U> _cache = new();

    public void Add(T key, U item)
    {
        _cache[key] = item;
    }

    public U? Get(T key)
    {
        if (_cache.TryGetValue(key, out var value))
            return value;

        return null;
    }

    public IEnumerable<U> GetMultiple(IEnumerable<T> keys)
    {
        foreach (var key in keys)
        {
            if (_cache.TryGetValue(key, out var value))
            {
                yield return value;
            }
        }
    }

    public IEnumerable<U> GetAll() => _cache.Values;

    public IEnumerable<T> GetKeys() => _cache.Keys;
}
