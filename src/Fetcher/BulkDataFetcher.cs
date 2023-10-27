using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Services;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Fetcher;

public class BulkDataFetcher<T, U> : DataFetcher, IBulkDataFetcher<T, U>
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

    public async Task<List<T>> FetchByIdsAsync(IEnumerable<int> ids, int? world = null)
    {
        var idList = ids.ToList();
        if (!idList.Any())
            return new List<T>();

        var batcher = new Batcher<int>(_entriesPerPage);
        var batches = batcher.SplitIntoBatchJobs(idList);

        var resultList = new List<T>();
        foreach (var batch in batches)
        {
            try
            {
                var response = await FetchAsync(world, batch);
                if (response is null)
                    continue;

                var content = response.GetContentAsList();
                resultList.AddRange(content);
            }
            catch (Exception e)
            {
                Logger.LogError($"Failed to fetch contents of batch: {e.Message}");
            }
        }

        return resultList;
    }

    private async Task<U> FetchAsync(int? world, List<int> batch)
    {
        var path = GetUrlPathFromEntries(batch, world);
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