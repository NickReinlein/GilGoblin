namespace GilGoblin.Web;

public abstract class DataFetcher<T, U>
    where T : IReponseToList<U>
    where U : class
{
    protected HttpClient Client { get; set; }

    protected string _basePath;

    public DataFetcher(string basePath, HttpClient? client = null)
    {
        _basePath = basePath;
        Client = client ?? new HttpClient();
        Client.Timeout = new TimeSpan(0, 0, 10);
    }

    protected virtual async Task<U?> GetAsync(string path)
    {
        var fullPath = string.Concat(_basePath, path);

        var response = await Client.GetAsync(fullPath);
        if (!response.IsSuccessStatusCode)
            return default;

        return await response.Content.ReadFromJsonAsync<U>();
    }

    protected virtual async Task<IEnumerable<U>> GetMultipleAsync(string path)
    {
        var fullPath = string.Concat(_basePath, path);

        var response = await Client.GetAsync(fullPath);
        if (!response.IsSuccessStatusCode)
            return new List<U>();

        var list = await response.Content.ReadFromJsonAsync<T>();
        return list?.GetContentAsList() ?? new List<U>();
    }
}
