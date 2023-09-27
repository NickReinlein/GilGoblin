using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using GilGoblin.Web;
using GilGoblin.Database;
using Microsoft.Extensions.Hosting;

namespace GilGoblin.DataUpdater;

public class DataUpdater<T, U> : BackgroundService
    where T : class
    where U : class, IReponseToList<T>
{
    protected readonly IDataFetcher<T, U> _fetcher;
    protected readonly GilGoblinDbContext _dbContext;
    protected readonly ILogger<DataUpdater<T, U>> _logger;

    public DataUpdater(
        GilGoblinDbContext dbContext,
        IDataFetcher<T, U> fetcher,
        ILogger<DataUpdater<T, U>> logger
    )
    {
        _dbContext = dbContext;
        _fetcher = fetcher;
        _logger = logger;
    }
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var updated = await FetchUpdateAsync();

                await SaveUpdatedAsync(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception occured during the Api call: {ex.Message}");
            }
            await Task.Delay(TimeSpan.FromMinutes(5), ct);
        }
    }

    protected virtual async Task<IEnumerable<T>> FetchUpdateAsync()
    {
        var entriesToUpdate = await GetEntriesToUpdateAsync();
        if (!entriesToUpdate.Any())
        {
            _logger.LogInformation($"No entries need to be updated for {nameof(T)}");
            return Enumerable.Empty<T>();
        }

        _logger.LogInformation($"Fetching updates for {entriesToUpdate.Count()} {nameof(T)} entries");
        return await FetchUpdateForEntries(entriesToUpdate);
    }

    protected async Task SaveUpdatedAsync(IEnumerable<T> updated)
    {
        if (!updated.Any())
            return;

        using var context = _dbContext;
        await context.AddRangeAsync(updated);
        await context.SaveChangesAsync();
        _logger.LogInformation($"Saved {updated.Count()} entries for type {typeof(T).Name}");
    }

    protected virtual async Task<IEnumerable<T>> FetchUpdateForEntries(
        IEnumerable<T> entriesToUpdate
    )
    {
        var apiUrl = await GetUrlPathFromEntries(entriesToUpdate);

        var response = await _fetcher.GetMultipleAsync(apiUrl);
        if (response is null)
        {
            _logger.LogError($"Failed to fetch the apiUrl: {apiUrl}");
            return Enumerable.Empty<T>();
        }

        return response.GetContentAsList();
    }

    protected virtual async Task<IEnumerable<T>> GetEntriesToUpdateAsync() => await Task.Run(() => Enumerable.Empty<T>());
    protected virtual async Task<string> GetUrlPathFromEntries(IEnumerable<T> entries) =>
        await Task.Run(() => string.Empty);
}
