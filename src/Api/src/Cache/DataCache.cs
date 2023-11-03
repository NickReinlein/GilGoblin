using System.Collections.Generic;

namespace GilGoblin.Api.Cache;

public class DataCache<T, U> : IDataCache<T, U>
    where U : class
{
    protected readonly Dictionary<T, U> Cache = new();

    public void Add(T key, U item)
    {
        Cache[key] = item;
    }

    public U? Get(T key) => Cache.TryGetValue(key, out var value) ? value : null;

    public IEnumerable<U> GetMultiple(IEnumerable<T> keys)
    {
        foreach (var key in keys)
        {
            if (Cache.TryGetValue(key, out var value))
            {
                yield return value;
            }
        }
    }

    public IEnumerable<U> GetAll() => Cache.Values;

    public IEnumerable<T> GetKeys() => Cache.Keys;
}