using System.Collections.Generic;

namespace GilGoblin.Api.Cache;

public interface ICalculatedMetricCache<T> where T : class
{
    T? Get((int, int, bool) key);
    void Add((int, int, bool) key, T item);
    void Delete((int, int, bool) key);
}

public class CalculatedMetricCache<T> : ICalculatedMetricCache<T> where T : class
{
    private static readonly Dictionary<(int, int, bool), T> _cache = new();
    public T? Get((int, int, bool) key) => _cache.GetValueOrDefault(key);
    public void Add((int, int, bool) key, T item) => _cache[key] = item;
    public void Delete((int, int, bool) key) => _cache.Remove(key, out _);
}