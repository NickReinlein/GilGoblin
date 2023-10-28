using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Fetcher;

public interface IDataFetcher<T> where T : class, IIdentifiable
{
    Task<List<T>> FetchByIdsAsync(CancellationToken ct, IEnumerable<int> ids, int? world = null);
}

public abstract class DataFetcher<T> : IDataFetcher<T> where T : class, IIdentifiable
{
    protected string BasePath { get; set; }
    protected HttpClient Client { get; set; }
    protected readonly ILogger<DataFetcher<T>> Logger;

    protected DataFetcher(
        string basePath,
        ILogger<DataFetcher<T>> logger,
        HttpClient? client = null)
    {
        BasePath = basePath;
        Logger = logger;
        Client = client ?? new HttpClient();
    }

    public abstract Task<List<T>> FetchByIdsAsync(CancellationToken ct, IEnumerable<int> ids, int? world = null);
}