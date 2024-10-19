using System.Collections.Generic;
using System.Linq;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Cache;

public interface IRecipeCostCache
{
    RecipeCostPoco? Get((int, int, bool) key);
    List<RecipeCostPoco> GetAll();
    void Add((int, int, bool) key, RecipeCostPoco item);
    void Delete((int, int, bool) key);

    // IEnumerable<U> GetMultiple(IEnumerable<(int, int, bool)> keys);
    // IEnumerable<T> GetKeys();
}

public class RecipeCostCache : IRecipeCostCache
{
    private static readonly Dictionary<(int, int, bool), RecipeCostPoco> _cache = new();
    public RecipeCostPoco? Get((int, int, bool) key)
    {
        return _cache.GetValueOrDefault(key);
    }

    public List<RecipeCostPoco> GetAll()
    {
        return _cache.Values.ToList();
    }

    public void Add((int, int, bool) key, RecipeCostPoco item)
    {
        _cache[key] = item;
    }

    public void Delete((int, int, bool) key)
    {
        var found =_cache.TryGetValue(key, out _);
        if (found)
            _cache.Remove(key);
    }
}