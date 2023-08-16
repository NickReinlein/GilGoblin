using System.Collections.Generic;

namespace GilGoblin.Cache;

public class DataCache<T> : IDataCache<T>
    where T : class
{
    protected Dictionary<int, T> _cache = new();

    public void Add(int id, T item)
    {
        _cache[id] = item;
    }

    public T? Get(int id)
    {
        if (_cache.TryGetValue(id, out var value))
            return value;

        return null;
    }

    public IEnumerable<T> GetMultiple(IEnumerable<int> ids)
    {
        foreach (var id in ids)
        {
            if (_cache.TryGetValue(id, out var value))
            {
                yield return value;
            }
        }
    }

    public IEnumerable<T> GetAll()
    {
        return _cache.Values;
    }
}
