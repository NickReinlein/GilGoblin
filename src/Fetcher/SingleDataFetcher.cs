using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Fetcher;

public abstract class SingleDataFetcher<T> : DataFetcher
    where T : class, IIdentifiable
{
    public SingleDataFetcher(
        string basePath,
        IMarketableItemIdsFetcher marketableFetcher,
        ILogger<SingleDataFetcher<T>> logger,
        HttpClient? client = null)
        : base(basePath, marketableFetcher, logger, client)
    {
    }

    public async Task<List<T>> FetchByIdsAsync(IEnumerable<int> ids, int? world = null)
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

            var data = ReadResponseContentAsync(response.Content);
            return data;
        }
        catch
        {
            Logger.LogError($"Failed GET call for {nameof(T)} with path: {path}");
            return null;
        }
    }

    protected virtual T ReadResponseContentAsync(HttpContent content)
    {
        return content.ReadFromJsonAsync<T>().Result;
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