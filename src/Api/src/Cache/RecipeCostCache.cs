using System.Collections.Generic;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Cache;

public interface IRecipeCostCache
{
    RecipeCostPoco? Get((int, int, bool) key);
    void Add((int, int, bool) key, RecipeCostPoco item);
    void Delete((int, int, bool) key);
}

public class RecipeCostCache : IRecipeCostCache
{
    private static readonly Dictionary<(int, int, bool), RecipeCostPoco> _cache = new();
    public RecipeCostPoco? Get((int, int, bool) key) => _cache.GetValueOrDefault(key);

    public void Add((int, int, bool) key, RecipeCostPoco item) => _cache[key] = item;

    public void Delete((int, int, bool) key) => _cache.Remove(key, out _);
}