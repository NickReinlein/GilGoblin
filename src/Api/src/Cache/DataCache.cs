using System.Collections.Concurrent;
using System.Collections.Generic;

namespace GilGoblin.Api.Cache;

public class DataCache<T, U> : IDataCache<T, U>
    where T : notnull
    where U : class
{
    protected readonly ConcurrentDictionary<T, U> Cache = new();

    public void Add(T key, U item)
    {
        Cache[key] = item;
    }

    public U? Get(T key) => Cache.GetValueOrDefault(key);

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