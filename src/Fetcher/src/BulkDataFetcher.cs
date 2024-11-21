using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Fetcher.Pocos;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Fetcher;

public class BulkDataFetcher<T, U>(
    string basePath,
    ILogger<BulkDataFetcher<T, U>> logger,
    HttpClient? client = null)
    : DataFetcher<T>(basePath, logger, client), IBulkDataFetcher<T, U>
    where T : class, IIdentifiable
    where U : class, IResponseToList<T>
{
    protected int _entriesPerPage = 100;

    public int GetEntriesPerPage() => _entriesPerPage;
    public void SetEntriesPerPage(int count) => _entriesPerPage = count;

    public override async Task<List<T>> FetchByIdsAsync(IEnumerable<int> ids, int? world = null, CancellationToken ct = default)
    {
        var idList = ids.ToList();
        if (!idList.Any())
            return new List<T>();

        try
        {
            var response = await FetchAsync(world, idList, ct);
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

    private async Task<U?> FetchAsync(int? world, IEnumerable<int> ids, CancellationToken ct)
    {
        var path = GetUrlPathFromEntries(ids, world);
        return await FetchAndSerializeDataAsync(path, ct);
    }

    private async Task<U?> FetchAndSerializeDataAsync(string path, CancellationToken ct)
    {
        try
        {
            var response = await Client.GetAsync(path, ct);
            return !response.IsSuccessStatusCode
                ? null
                : await ReadResponseContentAsync(response.Content);
        }
        catch (Exception e)
        {
            Logger.LogError("Failed the GET call to update {Name} with path {Path}: {Error}",
                nameof(T),
                path,
                e.Message);
            return null;
        }
    }

    protected virtual async Task<U> ReadResponseContentAsync(HttpContent content) =>
        JsonSerializer.Deserialize<U>(await content.ReadAsStringAsync(), GetJsonSerializerOptions()) ??
        throw new InvalidOperationException($"Failed to read the fetched response: {content}");

    private static JsonSerializerOptions GetJsonSerializerOptions() =>
        new() { PropertyNameCaseInsensitive = true, IncludeFields = true };

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