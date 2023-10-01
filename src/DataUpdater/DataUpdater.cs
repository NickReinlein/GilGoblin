using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using GilGoblin.Web;
using GilGoblin.Database;
using Microsoft.Extensions.Hosting;

namespace GilGoblin.DataUpdater;

public class DataUpdater<T, U> : BackgroundService
    where T : class, IIdentifiable
    where U : class, IResponseToList<T>
{
    protected readonly IDataFetcher<T> Fetcher;
    protected readonly IDataSaver<T> Saver;
    private readonly ILogger<DataUpdater<T, U>> _logger;

    public DataUpdater(IDataFetcher<T> fetcher, IDataSaver<T> saver, ILogger<DataUpdater<T, U>> logger)
    {
        Fetcher = fetcher;
        Saver = saver;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await SaveAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"An exception occured during the Api call: {ex.Message}");
            }

            await Task.Delay(TimeSpan.FromMinutes(5), ct);
        }
    }

    protected virtual async Task SaveAsync()
    {
        var batchesToUpdate = await GetEntriesToUpdateAsync();
        if (!batchesToUpdate.Any())
        {
            _logger.LogInformation($"No entries need to be updated for {nameof(T)}");
            return;
        }

        foreach (var batch in batchesToUpdate)
        {
            var updated = await FetchUpdates(batch);
            if (updated.Any())
                await Saver.SaveAsync(updated);
        }
    }

    private async Task<List<T>> FetchUpdates(IReadOnlyCollection<T> entriesToUpdate)
    {
        var idsToUpdate = entriesToUpdate.Select(i => i.GetId());
        _logger.LogInformation($"Fetching updates for {entriesToUpdate.Count} {nameof(T)} entries");
        var updated = await FetchUpdateAsync(idsToUpdate);
        return updated;
    }

    private async Task<List<T>> FetchUpdateAsync(IEnumerable<int> entriesToUpdate)
    {
        var entriesList = entriesToUpdate.ToList();
        if (!entriesList.Any())
        {
            _logger.LogInformation($"No entries need to be updated for {nameof(T)}");
            return new List<T>();
        }

        var response = await FetchUpdatesForIDsAsync(entriesList);
        if (response is not null)
            return response.ToList();

        _logger.LogError($"Failed to fetch updates for {entriesList.Count} entries of {nameof(T)}");
        return new List<T>();
    }

    private async Task<IEnumerable<T>> FetchUpdatesForIDsAsync(IEnumerable<int> idsToUpdate, int worldId = 0)
    {
        try
        {
            var timer = new Stopwatch();
            timer.Start();
            var updated = await Fetcher.FetchByIdsAsync(idsToUpdate, worldId);
            timer.Stop();
            var callTime = timer.Elapsed.TotalMilliseconds;

            var worldString = worldId > 0 ? $"for world {worldId}" : string.Empty;
            _logger.LogInformation($"Received updates for {updated.Count} {nameof(T)} entries {worldString}");
            _logger.LogInformation($"Total call time: {callTime}");
            return updated;
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to fetch updates for {nameof(T)}: {e.Message}");
            return Enumerable.Empty<T>();
        }
    }

    protected virtual async Task<List<List<T>>> GetEntriesToUpdateAsync() =>
        await Task.Run(() => new List<List<T>>());
}