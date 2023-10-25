using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Fetcher;

public abstract class BulkDataFetcher<T, U> : DataFetcher, IBulkDataFetcher<T, U>
    where T : class, IIdentifiable
    where U : class, IResponseToList<T>
{
    public BulkDataFetcher(string basePath, ILogger<BulkDataFetcher<T, U>> logger, HttpClient? client = null)
        : base(basePath, logger, client)
    {
    }

    public async Task<List<T>> FetchByIdsAsync(IEnumerable<int> ids, int? world = null)
    {
        var idList = ids.ToList();
        if (!idList.Any())
            return new List<T>();

        var path = GetUrlPathFromEntries(idList, world);

        var response = await FetchAndSerializeDataAsync(path);

        return response is not null ? response.GetContentAsList() : new List<T>();
    }

    private async Task<U?> FetchAndSerializeDataAsync(string path)
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
            Logger.LogError($"Failed GET call to update {nameof(T)} with path: {path}");
            return null;
        }
    }

    protected virtual U ReadResponseContentAsync(HttpContent content)
    {
        return content.ReadFromJsonAsync<U>().Result;
    }

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