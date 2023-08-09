using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GilGoblin.Web;

public abstract class DataFetcher<T, U> : IDataFetcher<T, U>
    where T : class
    where U : class
{
    protected readonly string BasePath;
    protected HttpClient Client { get; set; }

    public DataFetcher(string basePath)
    {
        BasePath = basePath;
        Client = new HttpClient();
    }

    public DataFetcher(string basePath, HttpClient client)
    {
        BasePath = basePath;
        Client = client;
    }

    public virtual async Task<T?> GetAsync(string path) =>
        await FetchAndSerializeDataAsync<T?>(path);

    public virtual async Task<U?> GetMultipleAsync(string path) =>
        await FetchAndSerializeDataAsync<U?>(path);

    private async Task<F?> FetchAndSerializeDataAsync<F>(string path)
    {
        var response = await PerformGetAsync(path);
        if (!response.IsSuccessStatusCode)
            return default;

        try
        {
            return await response.Content.ReadFromJsonAsync<F>();
        }
        catch
        {
            return default;
        }
    }

    private async Task<HttpResponseMessage> PerformGetAsync(string path)
    {
        var fullPath = string.Concat(BasePath, path);
        return await Client.GetAsync(fullPath);
    }
}
