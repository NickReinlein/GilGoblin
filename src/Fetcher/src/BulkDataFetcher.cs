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

public class BulkDataFetcher<T, U> : DataFetcher<T>, IBulkDataFetcher<T, U>
    where T : class, IIdentifiable
    where U : class, IResponseToList<T>
{
    protected int _entriesPerPage = 100;

    public BulkDataFetcher(
        string basePath,
        ILogger<BulkDataFetcher<T, U>> logger,
        HttpClient? client = null
    )
        : base(basePath, logger, client)
    {
    }

    public int GetEntriesPerPage() => _entriesPerPage;
    public void SetEntriesPerPage(int count) => _entriesPerPage = count;

    public override async Task<List<T>> FetchByIdsAsync(CancellationToken ct, IEnumerable<int> ids, int? world = null)
    {
        var idList = ids.ToList();
        if (!idList.Any())
            return new List<T>();

        try
        {
            var response = await FetchAsync(world, ids);
            if (response is null)
                return new List<T>();

            var content = response.GetContentAsList();
            return content;
        }
        catch (Exception e)
        {
            Logger.LogError($"Failed to fetch contents of batch: {e.Message}");
            return new List<T>();
        }
    }

    private async Task<U> FetchAsync(int? world, IEnumerable<int> ids)
    {
        var path = GetUrlPathFromEntries(ids, world);
        var response = await FetchAndSerializeDataAsync(path);
        return response;
    }

    private async Task<U?> FetchAndSerializeDataAsync(string path)
    {
        try
        {
            var response = await Client.GetAsync(path);
            return !response.IsSuccessStatusCode
                ? null
                : ReadResponseContentAsync(response.Content);
        } 
        catch
        {
            Logger.LogError($"Failed GET call to update {nameof(T)} with path: {path}");
            return null;
        }
    }

    protected virtual U ReadResponseContentAsync(HttpContent content)
        => content.ReadFromJsonAsync<U>().Result;

    protected virtual string GetUrlPathFromEntries(IEnumerable<int> ids, int? worldId = null)
    {
        var sb = new StringBuilder();
        sb.Append(BasePath);

        if (worldId is not null)
            sb.Append($"{worldId}/");

        var idList = ids.ToList();

        foreach (var id in idList)
        {
            sb.Append(id);
            if (id != idList.Last())
                sb.Append(',');
        }

        return sb.ToString();
    }
}