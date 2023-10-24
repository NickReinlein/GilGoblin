using System.Net.Http;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Fetcher;

public abstract class DataFetcher
{
    protected string BasePath { get; set; }
    protected HttpClient Client { get; set; }
    protected ILogger<DataFetcher> Logger;
    protected DataFetcher(string basePath, ILogger<DataFetcher> logger, HttpClient? client = null)
    {
        BasePath = basePath;
        Logger = logger;
        Client = client ?? new HttpClient();
    }

}