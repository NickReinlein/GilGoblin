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

public abstract class DataUpdater<T, U>(
    IServiceProvider serviceProvider,
    ILogger<DataUpdater<T, U>> logger)
    : BackgroundService, IDataUpdater<T, U>
    where T : class, IIdentifiable
    where U : class, IIdentifiable
{
    protected readonly ILogger<DataUpdater<T, U>> Logger = logger;

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

    protected abstract Task ExecuteUpdateAsync(CancellationToken ct);

    protected abstract Task ConvertAndSaveToDbAsync(List<U> updated, int? worldId = null);

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

            using var scope = serviceProvider.CreateScope();
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

    protected abstract Task<List<int>> GetIdsToUpdateAsync(int? worldId);
    protected virtual int GetApiSpamDelayInMs() => 60000;
}