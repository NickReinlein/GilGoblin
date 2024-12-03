using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Fetcher;

public interface ISingleDataFetcher<T> : IDataFetcher<T>
    where T : class, IIdentifiable
{
    Task<T?> FetchByIdAsync(int id, int? world = null);
}

public abstract class SingleDataFetcher<T>(
    string basePath,
    ILogger<SingleDataFetcher<T>> logger,
    HttpClient? client = null)
    : DataFetcher<T>(basePath, logger, client), ISingleDataFetcher<T>
    where T : class, IIdentifiable
{
    public override async Task<List<T>> FetchByIdsAsync(IEnumerable<int> ids, int? world = null, CancellationToken ct = default)
    {
        var result = new List<T>();
        foreach (var id in ids.ToList())
        {
            var fetched = await FetchByIdAsync(id, world);
            if (fetched is not null)
                result.Add(fetched);
        }

        return result;
    }

    public async Task<T?> FetchByIdAsync(int id, int? world = null)
    {
        if (id < 1)
            return null;

        var path = GetUrlPathFromEntry(id, world);

        return await FetchAndSerializeDataAsync(path);
    }

    protected async Task<T?> FetchAndSerializeDataAsync(string path)
    {
        try
        {
            var response = await Client.GetAsync(path);
            if (!response.IsSuccessStatusCode)
                return null;

            var result = await response.Content.ReadFromJsonAsync<T>() ?? throw new InvalidOperationException();
            return result;
        }
        catch
        {
            Logger.LogError($"Failed GET call for {typeof(T).Name} with path: {path}");
            return null;
        }
    }

    protected virtual string GetUrlPathFromEntry(int id, int? worldId = null)
    {
        var sb = new StringBuilder();
        sb.Append(BasePath);

        if (worldId is not null)
            sb.Append($"{worldId}/");

        sb.Append(id);

        return sb.ToString();
    }
}