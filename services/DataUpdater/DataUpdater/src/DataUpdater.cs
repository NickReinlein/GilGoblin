using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Extensions.Logging;
using GilGoblin.Fetcher;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GilGoblin.DataUpdater;

public interface IDataUpdater<T, U>
    where T : class, IIdentifiable
    where U : class, IIdentifiable
{
    Task FetchAsync(CancellationToken ct, int? worldId = null);
}

public abstract class DataUpdater<T, U> : BackgroundService, IDataUpdater<T, U>
    where T : class, IIdentifiable
    where U : class, IIdentifiable
{
    protected readonly IServiceScopeFactory ScopeFactory;
    protected readonly ILogger<DataUpdater<T, U>> Logger;

    public DataUpdater(
        IServiceScopeFactory scopeFactory,
        ILogger<DataUpdater<T, U>> logger)
    {
        ScopeFactory = scopeFactory;
        Logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                var worldId = GetWorldId();
                var worldIdString = worldId is null ? string.Empty : $" for world {worldId.ToString()}";
                Logger.LogInformation($"Fetching updates of type {typeof(T)}{worldIdString}");
                await FetchAsync(ct, worldId);
            }
            catch (Exception ex)
            {
                Logger.LogError($"An exception occured during the Api call: {ex.Message}");
            }

            Logger.LogDebug("Awaiting delay before making another update");
            await Task.Delay(TimeSpan.FromMinutes(5), ct);
        }
    }

    protected abstract Task ConvertAndSaveToDbAsync(List<U> updated);

    public async Task FetchAsync(CancellationToken ct, int? worldId)
    {
        var idList = await GetIdsToUpdateAsync(worldId);
        if (!idList.Any())
            return;

        await FetchUpdatesAsync(worldId, idList, ct);
    }

    protected virtual async Task FetchUpdatesAsync(int? worldId, List<int> idList, CancellationToken ct)
    {
        var updated = await FetchUpdatesForIdsAsync(idList, worldId, ct);
        if (updated.Any())
            await ConvertAndSaveToDbAsync(updated);
    }

    private async Task<List<U>> FetchUpdatesForIdsAsync(IEnumerable<int> idsToUpdate,
        int? worldId, CancellationToken ct)
    {
        try
        {
            var idList = idsToUpdate.ToList();
            using var scope = ScopeFactory.CreateScope();
            var fetcher = scope.ServiceProvider.GetRequiredService<IDataFetcher<U>>();
            var worldString = worldId > 0 ? $"for world {worldId}" : string.Empty;
            Logger.LogInformation($"Fetching updates for {idList.Count} {nameof(T)} {worldString}");
            var timer = new Stopwatch();
            timer.Start();
            var updated = await fetcher.FetchByIdsAsync(ct, idList, worldId);
            timer.Stop();
            var callTime = timer.Elapsed.TotalMilliseconds;

            Logger.LogInformation($"Received updates for {updated.Count} {nameof(T)} entries {worldString}");
            Logger.LogInformation($"Total call time: {callTime}");
            return updated;
        }
        catch (Exception e)
        {
            Logger.LogError($"Failed to fetch updates for {nameof(T)}: {e.Message}");
            return new List<U>();
        }
    }

    protected virtual int? GetWorldId() => null;

    protected abstract Task<List<int>> GetIdsToUpdateAsync(int? worldId);
    protected virtual int GetApiSpamDelayInMs() => 5000;
}