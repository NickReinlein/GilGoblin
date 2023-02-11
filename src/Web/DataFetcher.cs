namespace GilGoblin.Web;

public abstract class DataFetcher<T>
{
    protected HttpClient Client { get; set; }

    protected string _basePath;

    public DataFetcher(string basePath, HttpClient? client = null)
    {
        _basePath = basePath;
        Client = client ?? new HttpClient();
        Client.Timeout = new TimeSpan(0, 0, 10);
    }

    protected virtual async Task<T?> GetAsync(string path)
    {
        var fullPath = string.Concat(_basePath, path);
        var response = await Client.GetAsync(fullPath);
        if (!response.IsSuccessStatusCode)
            return default;

        var result = await response.Content.ReadAsAsync<T>();
        return result;
    }

    protected virtual async Task<IEnumerable<T>> GetMultipleAsync(string path)
    {
        var fullPath = string.Concat(_basePath, path);
        var response = await Client.GetAsync(fullPath);
        if (!response.IsSuccessStatusCode)
            return Array.Empty<T>();

        return await response.Content.ReadAsAsync<IEnumerable<T>>() ?? Array.Empty<T>();
    }
}
