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

// ReSharper disable twice UnusedTypeParameter
public interface IDataUpdater<T, U>
    where T : class, IIdentifiable
    where U : class, IIdentifiable
{
    Task FetchAsync(int? worldId = null, CancellationToken ct = default);
}

public abstract class DataUpdater<T, U> : BackgroundService, IDataUpdater<T, U>
    where T : class, IIdentifiable
    where U : class, IIdentifiable
{
    protected readonly IServiceScopeFactory ScopeFactory;
    protected readonly ILogger<DataUpdater<T, U>> Logger;

    protected DataUpdater(
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
                await ExecuteUpdateAsync(ct);
            }
            catch (Exception ex)
            {
                Logger.LogError($"An exception occured during the Api call: {ex.Message}");
            }

            var delay = TimeSpan.FromMinutes(5);
            Logger.LogInformation($"Awaiting delay of {delay.TotalSeconds} seconds before performing next update");
            await Task.Delay(delay, ct);
        }
    }

    protected virtual async Task ExecuteUpdateAsync(CancellationToken ct)
    {
        var worlds = GetWorlds();
        var worldIdString = !worlds.Any() ? string.Empty : $" for world {worlds}";
        Logger.LogInformation($"Fetching updates of type {typeof(T)}{worldIdString}");
        await FetchAsync(worlds.FirstOrDefault()?.GetId(), ct);
    }

    protected virtual Task ConvertAndSaveToDbAsync(List<U> updated) => Task.CompletedTask;

    public async Task FetchAsync(int? worldId = null, CancellationToken ct = default)
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

    private async Task<List<U>> FetchUpdatesForIdsAsync(
        IEnumerable<int> idsToUpdate,
        int? worldId,
        CancellationToken ct)
    {
        try
        {
            var idList = idsToUpdate.ToList();

            using var scope = ScopeFactory.CreateScope();
            var fetcher = scope.ServiceProvider.GetRequiredService<IDataFetcher<U>>();
            var worldString = worldId > 0 ? $"for world id {worldId}" : string.Empty;
            Logger.LogInformation($"Fetching updates for {idList.Count} {nameof(T)} {worldString}");
            var timer = new Stopwatch();
            timer.Start();
            var updated = await fetcher.FetchByIdsAsync(idList, worldId, ct);
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

    protected virtual List<WorldPoco> GetWorlds() => new();

    protected virtual Task<List<int>> GetIdsToUpdateAsync(int? worldId) => Task.FromResult(new List<int>());
    protected virtual int GetApiSpamDelayInMs() => 5000;
}