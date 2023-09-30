using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using GilGoblin.DataUpdater;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Web;

public abstract class DataFetcher<T, U> : IDataFetcher<T>
    where T : class, IIdentifiable
    where U : class, IResponseToList<T>
{
    protected string BasePath { get; set; }
    protected HttpClient Client { get; set; }
    private readonly ILogger<DataFetcher<T, U>> _logger;

    public DataFetcher(string basePath, ILogger<DataFetcher<T, U>> logger, HttpClient? client = null)
    {
        BasePath = basePath;
        Client = client ?? new HttpClient();
        _logger = logger;
    }

    public async Task<List<T>> FetchByIdsAsync(IEnumerable<int> ids, int? world = null)
    {
        var idList = ids.ToList();
        if (!idList.Any())
            return new List<T>();

        var path = GetUrlPathFromEntries(idList);

        var response = await FetchAndSerializeDataAsync(path);

        return response is not null ? response.GetContentAsList() : new List<T>();
    }

    public async Task<U?> FetchAndSerializeDataAsync(string path)
    {
        try
        {
            var response = await Client.GetAsync(path);
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<U>()
                : null;
        }
        catch
        {
            _logger.LogError($"Failed GET call to update {nameof(T)} with path: {path}");
            return null;
        }
    }

    protected virtual string GetUrlPathFromEntries(IEnumerable<int> ids) => BasePath;
}