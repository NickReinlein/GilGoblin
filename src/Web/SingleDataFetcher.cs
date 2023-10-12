using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.DataUpdater;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Web;

public abstract class SingleDataFetcher<T> : ISingleDataFetcher<T>
    where T : class, IIdentifiable
{
    protected string BasePath { get; set; }
    protected HttpClient Client { get; set; }
    private readonly ILogger<SingleDataFetcher<T>> _logger;

    public SingleDataFetcher(string basePath, ILogger<SingleDataFetcher<T>> logger, HttpClient? client = null)
    {
        BasePath = basePath;
        Client = client ?? new HttpClient();
        _logger = logger;
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

            return await response.Content.ReadFromJsonAsync<T>();
        }
        catch
        {
            _logger.LogError($"Failed GET call for {nameof(T)} with path: {path}");
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