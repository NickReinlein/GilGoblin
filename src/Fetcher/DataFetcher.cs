using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Fetcher;

public interface IDataFetcher<T> where T : class, IIdentifiable
{
    Task<List<T>> FetchByIdsAsync(IEnumerable<int> ids, int? world = null);
}

public abstract class DataFetcher
{
    protected string BasePath { get; set; }
    protected HttpClient Client { get; set; }
    protected IMarketableItemIdsFetcher MarketableFetcher { get; set; }
    protected readonly ILogger<DataFetcher> Logger;

    protected DataFetcher(
        string basePath,
        IMarketableItemIdsFetcher marketableFetcher,
        ILogger<DataFetcher> logger,
        HttpClient? client = null)
    {
        BasePath = basePath;
        Logger = logger;
        Client = client ?? new HttpClient();
        MarketableFetcher = marketableFetcher;
    }
}