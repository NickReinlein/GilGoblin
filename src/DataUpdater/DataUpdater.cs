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

public interface IDataUpdater<T, U>
    where T : class, IIdentifiable
    where U : class, IIdentifiable
{
    Task FetchAsync(int? worldId = null);
}

public abstract class DataUpdater<T, U> : BackgroundService, IDataUpdater<T, U>
    where T : class, IIdentifiable
    where U : class, IIdentifiable
{
    protected readonly IDataSaver<T> Saver;
    protected readonly IDataFetcher<U> Fetcher;
    protected readonly ILogger<DataUpdater<T, U>> Logger;

    public DataUpdater(IDataSaver<T> saver, IDataFetcher<U> fetcher, ILogger<DataUpdater<T, U>> logger)
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
            await Task.Delay(TimeSpan.FromSeconds(60), ct);
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
                await ConvertToDbFormatAndSave(updated);

            await Task.Delay(GetApiSpamDelayInMs());
        }
    }

    protected abstract Task ConvertToDbFormatAndSave(List<U> updated);

    private async Task<List<U>> FetchUpdatesForEntriesAsync(IEnumerable<int> entriesToUpdate, int? worldId)
    {
        var entriesList = entriesToUpdate.ToList();
        if (!entriesList.Any())
        {
            Logger.LogInformation($"No entries need to be updated for {nameof(T)}");
            return new List<U>();
        }

        var response = await FetchUpdatesForIdsAsync(entriesList, worldId);
        if (response is not null)
            return response.ToList();

        Logger.LogError($"Failed to fetch updates for {entriesList.Count} entries of {nameof(T)}");
        return new List<U>();
    }

    private async Task<IEnumerable<U>> FetchUpdatesForIdsAsync(IEnumerable<int> idsToUpdate, int? worldId)
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
            return Enumerable.Empty<U>();
        }
    }

    protected virtual int GetApiSpamDelayInMs() => 300;
    protected virtual int? GetWorldId() => null;

    protected abstract Task<List<List<int>>> GetIdsToUpdateAsync(int? worldId);
}