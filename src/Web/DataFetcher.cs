using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace GilGoblin.Web;

public abstract class DataFetcher<T, U> : IDataFetcher<T, U>
    where T : class
    where U : class
{
    protected HttpClient Client { get; set; } = new HttpClient();
    protected string _basePath;

    public DataFetcher(string basePath)
    {
        if (Client.Timeout == TimeSpan.Zero)
            Client.Timeout = new TimeSpan(0, 0, 10);
        _basePath = basePath;
    }

    public virtual async Task<T?> GetAsync(string path)
    {
        var fullPath = string.Concat(_basePath, path);

        var response = await Client.GetAsync(fullPath);
        if (!response.IsSuccessStatusCode)
            return default;

        return await response.Content.ReadFromJsonAsync<T>();
    }

    public virtual async Task<U?> GetMultipleAsync(string path)
    {
        var fullPath = string.Concat(_basePath, path);

        var response = await Client.GetAsync(fullPath);
        if (!response.IsSuccessStatusCode)
            return default;

        return await response.Content.ReadFromJsonAsync<U>();
    }
}
