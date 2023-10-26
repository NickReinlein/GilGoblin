using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using GilGoblin.Fetcher;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.Hosting;

namespace GilGoblin.DataUpdater;

public interface IDataUpdater<T> where T : class, IIdentifiable
{
    Task FetchAsync(int? worldId = null);
}

public abstract class DataUpdater<T> : BackgroundService, IDataUpdater<T>
    where T : class, IIdentifiable
{
    protected readonly IDataFetcher<T> Fetcher;
    protected readonly IDataSaver<T> Saver;
    protected readonly ILogger<DataUpdater<T>> Logger;

    public DataUpdater(IDataFetcher<T> fetcher, IDataSaver<T> saver, ILogger<DataUpdater<T>> logger)
    {
        Fetcher = fetcher;
        Saver = saver;
        Logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var worldId = GetWorldId();
                var worldIdString = worldId is null ? "" : worldId.ToString();
                Logger.LogInformation($"Fetching updates for {nameof(T)}{worldIdString}");
                await FetchAsync(worldId);
            }
            catch (Exception ex)
            {
                Logger.LogError($"An exception occured during the Api call: {ex.Message}");
            }

            Logger.LogDebug("Awaiting delay before making another update");
            // await Task.Delay(TimeSpan.FromMinutes(5), ct);
            await Task.Delay(TimeSpan.FromSeconds(20), ct);
        }
    }


    public async Task FetchAsync(int? worldId)
    {
        var idBatches = await GetIdsToUpdateAsync(worldId);
        if (!idBatches.Any())
            return;

        foreach (var idBatch in idBatches)
        {
            var updated = await FetchUpdatesForEntriesAsync(idBatch, worldId);
            if (updated.Any())
                await Saver.SaveAsync(updated);
        }
    }

    private async Task<List<T>> FetchUpdatesForEntriesAsync(IEnumerable<int> entriesToUpdate, int? worldId)
    {
        var entriesList = entriesToUpdate.ToList();
        if (!entriesList.Any())
        {
            Logger.LogInformation($"No entries need to be updated for {nameof(T)}");
            return new List<T>();
        }

        var response = await FetchUpdatesForIdsAsync(entriesList, worldId);
        if (response is not null)
            return response.ToList();

        Logger.LogError($"Failed to fetch updates for {entriesList.Count} entries of {nameof(T)}");
        return new List<T>();
    }

    private async Task<IEnumerable<T>> FetchUpdatesForIdsAsync(IEnumerable<int> idsToUpdate, int? worldId)
    {
        try
        {
            var worldString = worldId > 0 ? $"for world {worldId}" : string.Empty;
            var idList = idsToUpdate.ToList();
            Logger.LogInformation($"Fetching update for {idList.Count} {nameof(T)} {worldString}");
            var timer = new Stopwatch();
            timer.Start();
            var updated = await Fetcher.FetchByIdsAsync(idList, worldId);
            timer.Stop();
            var callTime = timer.Elapsed.TotalMilliseconds;

            Logger.LogInformation($"Received updates for {updated.Count} {nameof(T)} entries {worldString}");
            Logger.LogInformation($"Total call time: {callTime}");
            return updated;
        }
        catch (Exception e)
        {
            Logger.LogError($"Failed to fetch updates for {nameof(T)}: {e.Message}");
            return Enumerable.Empty<T>();
        }
    }

    protected virtual int? GetWorldId() => null;

    protected abstract Task<List<List<int>>> GetIdsToUpdateAsync(int? worldId);
}