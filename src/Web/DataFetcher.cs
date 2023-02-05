namespace GilGoblin.Web;

public class DataFetcher<T> where T : class
{
    public static HttpClient Client { get; set; } = new();

    protected string _basePath;

    public DataFetcher(string basePath)
    {
        _basePath = basePath;
        Client.Timeout = new TimeSpan(0, 0, 10);
    }

    public virtual async Task<T?> GetAsync(string path)
    {
        var fullPath = string.Concat(_basePath, path);

        var response = await Client.GetAsync(fullPath);
        if (!response.IsSuccessStatusCode)
            return null;

        var result = await response.Content.ReadFromJsonAsync<T>();
        return result;
    }

    public virtual async Task<IEnumerable<T>> GetAllAsync(string path)
    {
        var fullPath = string.Concat(_basePath, path);

        var response = await Client.GetAsync(fullPath);
        if (!response.IsSuccessStatusCode)
            return Array.Empty<T>();

        return await response.Content.ReadFromJsonAsync<IEnumerable<T>>() ?? Array.Empty<T>();
    }
}
