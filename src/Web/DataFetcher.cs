namespace GilGoblin.Web;

public abstract class DataFetcher<T, U> : IDataFetcher<T, U>
    where T : class
    where U : class
{
    protected HttpClient Client { get; set; }
    protected string _basePath;
    private readonly ILogger<DataFetcher<T, U>> _logger;

    public DataFetcher(string basePath, HttpClient? client, ILogger<DataFetcher<T, U>> logger)
    {
        Client = client ?? new HttpClient();
        Client.Timeout = new TimeSpan(0, 0, 10);
        _basePath = basePath;
        _logger = logger;
    }

    public virtual async Task<T?> GetAsync(string path)
    {
        var fullPath = string.Concat(_basePath, path);
        _logger.LogInformation("Beginning call to API endpoint: {Path}", path);

        var response = await Client.GetAsync(fullPath);
        if (!response.IsSuccessStatusCode)
            return default;

        _logger.LogInformation("Response received from API endpoint: {Path}", path);
        return await response.Content.ReadFromJsonAsync<T>();
    }

    public virtual async Task<U?> GetMultipleAsync(string path)
    {
        var fullPath = string.Concat(_basePath, path);
        _logger.LogInformation("Beginning call to API endpoint: {Path}", path);

        var response = await Client.GetAsync(fullPath);
        if (!response.IsSuccessStatusCode)
            return default;

        _logger.LogInformation("Response received from API endpoint: {Path}", path);
        return await response.Content.ReadFromJsonAsync<U>();
    }
}
