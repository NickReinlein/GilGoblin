using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Fetcher;

public interface IDataFetcher<T> where T : class, IIdentifiable
{
    Task<List<T>> FetchByIdsAsync(IEnumerable<int> ids, int? world = null, CancellationToken ct = default);
}

public abstract class DataFetcher<T>(
    string basePath,
    ILogger<DataFetcher<T>> logger,
    HttpClient? client = null)
    : IDataFetcher<T>
    where T : class, IIdentifiable
{
    protected string BasePath { get; init; } = basePath;
    protected HttpClient Client { get; init; } = client ?? new HttpClient();
    protected readonly ILogger<DataFetcher<T>> Logger = logger;

    public abstract Task<List<T>> FetchByIdsAsync(
        IEnumerable<int> ids,
        int? world = null,
        CancellationToken ct = default);
}