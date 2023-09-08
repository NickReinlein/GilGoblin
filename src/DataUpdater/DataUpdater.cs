using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using GilGoblin.Web;

namespace GilGoblin.Services.DataUpdater;

public interface IDataUpdater<T> where T : class
{
    Task UpdateAsync();
    Task GetEntriesToUpdateAsync(DbContext dbContext);
}

public class DataUpdater<T, U> : IDataUpdater<T> where T : class where U : class
{
    private readonly Timer timer;
    private readonly IBatcher<T> _batcher;
    private readonly IDataFetcher<T, U> _fetcher;
    private readonly ILogger<DataUpdater<T, U>> _logger;

    public DataUpdater(IBatcher<T> batcher, IDataFetcher<T, U> fetcher, ILogger<DataUpdater<T, U>> logger)
    {
        _batcher = batcher;
        _fetcher = fetcher;
        _logger = logger;
        timer = new Timer(async _ => await UpdateAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(5));
    }

    public virtual async Task GetEntriesToUpdateAsync(DbContext dbContext) => throw new NotImplementedException();

    public async Task UpdateAsync()
    {
        try
        {
            var entries = new List<T>();
            var batchJobs = _batcher.SplitIntoBatchJobs(entries);

            foreach (var job in batchJobs)
            {
                var response = await _fetcher.GetMultipleAsync("");

                if (!response?.IsSuccessStatusCode)
                {
                    _logger.LogError($"API call failed with status code: {response.StatusCode}");
                    continue;
                }

                var responseBody = await response.Content.ReadAsStringAsync();

                if (responseBody is not null) // add safety checks
                    SaveUpdateResult(responseBody);
            }

        }
        catch (Exception ex)
        {
            _logger.LogError($"An exception occured during the Api call with error message: {ex.message}");
        }
    }

    public async Task SaveUpdateResult(DbContext dbContext, IEnumerable<T> results)
    {
        if (!results.Any())
            return;

        using var context = dbContext;
        await context.AddRangeAsync(results);
        await context.SaveChangesAsync();
        _logger.LogInformation($"Saved {results.Count} entries for type {typeof(T).Name}");
    }
}
