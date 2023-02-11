namespace GilGoblin.Web;

public abstract class DataFetcher<T>
{
    public static HttpClient Client { get; set; } = new();

    protected string _basePath;

    public DataFetcher(string basePath)
    {
        _basePath = basePath;
        Client.Timeout = new TimeSpan(0, 0, 10);
    }

    protected virtual async Task<T?> GetAsync(string path)
    {
        var fullPath = string.Concat(_basePath, path);

        var response = await Client.GetAsync(fullPath);
        if (!response.IsSuccessStatusCode)
            return default;

        var result = await response.Content.ReadFromJsonAsync<T>();
        return result;
    }

    protected virtual async Task<IEnumerable<T>> GetMultipleAsync(string path)
    {
        var fullPath = string.Concat(_basePath, path);

        var response = await Client.GetAsync(fullPath);
        if (!response.IsSuccessStatusCode)
            return Array.Empty<T>();

        return await response.Content.ReadFromJsonAsync<IEnumerable<T>>() ?? Array.Empty<T>();
    }
}
