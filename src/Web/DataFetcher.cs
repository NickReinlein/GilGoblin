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

    public virtual async Task<T?> GetAsync(string path)
    {
        var response = await PerformGetAsync(path);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<T>()
            : default;
    }

    public virtual async Task<U?> GetMultipleAsync(string path)
    {
        var response = await PerformGetAsync(path);
        return response.IsSuccessStatusCode
            ? await response.Content.ReadFromJsonAsync<U>()
            : default;
    }

    private async Task<HttpResponseMessage> PerformGetAsync(string path)
    {
        var fullPath = string.Concat(BasePath, path);
        return await Client.GetAsync(fullPath);
    }
}
