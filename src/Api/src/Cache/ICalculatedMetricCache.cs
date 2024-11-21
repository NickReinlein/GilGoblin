using System.Collections.Generic;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Cache;

public interface ICalculatedMetricCache<T> where T : class
{
    T? Get(TripleKey key);
    void Add(TripleKey key, T item);
    void Delete(TripleKey key);
}

public class CalculatedMetricCache<T> : ICalculatedMetricCache<T> where T : class
{
    private static readonly Dictionary<TripleKey, T> _cache = new();
    public T? Get(TripleKey key) => _cache.GetValueOrDefault(key);
    public void Add(TripleKey key, T item) => _cache[key] = item;
    public void Delete(TripleKey key) => _cache.Remove(key, out _);
}