using System.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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

[ExcludeFromCodeCoverage]
public abstract class DataUpdater<T, U>(
    IServiceProvider serviceProvider,
    ILogger<DataUpdater<T, U>> logger)
    : BackgroundService, IDataUpdater<T, U>
    where T : class, IIdentifiable
    where U : class, IIdentifiable
{
    protected readonly IServiceProvider ServiceProvider =
        serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

    protected readonly ILogger<DataUpdater<T, U>> Logger = logger;

    public async Task FetchAsync(int? worldId = null, CancellationToken ct = default)
    {
        var idList = await GetIdsToUpdateAsync(worldId, ct);
        if (!idList.Any())
            return;

        await FetchUpdatesAsync(worldId, idList, ct);
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
                Logger.LogError("An exception occured during the Api call: {ExMessage}", ex.Message);
            }

            var delay = TimeSpan.FromMinutes(5);
            Logger.LogInformation("Awaiting delay of {DelayTotalSeconds} seconds before performing next update", delay.TotalSeconds);
            await Task.Delay(delay, ct);
        }
    }

    protected abstract Task ExecuteUpdateAsync(CancellationToken ct);

    protected abstract Task ConvertAndSaveToDbAsync(List<U> updated, int? worldId = null, CancellationToken ct = default);

    protected virtual async Task FetchUpdatesAsync(int? worldId, List<int> idList, CancellationToken ct)
    {
        var updated = await FetchUpdatesForIdsAsync(idList, worldId, ct);
        if (updated.Any())
            await ConvertAndSaveToDbAsync(updated, worldId, ct);
    }

    private async Task<List<U>> FetchUpdatesForIdsAsync(
        IEnumerable<int> idsToUpdate,
        int? worldId,
        CancellationToken ct)
    {
        try
        {
            var idList = idsToUpdate.ToList();

            using var scope = ServiceProvider.CreateScope();
            var fetcher = scope.ServiceProvider.GetRequiredService<IDataFetcher<U>>();
            var worldString = worldId > 0 ? $"for world id {worldId}" : string.Empty;
            Logger.LogInformation("Fetching updates for {IdListCount} {IIdentifiableName} {WorldString}", idList.Count, nameof(T), worldString);
            var timer = new Stopwatch();
            timer.Start();
            var updated = await fetcher.FetchByIdsAsync(idList, worldId, ct);
            timer.Stop();
            var callTime = timer.Elapsed.TotalMilliseconds;

            Logger.LogInformation(
                "Received updates for {Count} {Name} entries {WorldString}, total call time: {CallTime}",
                updated.Count,
                nameof(T),
                worldString,
                callTime);
            return updated;
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to fetch updates for {IIdentifiableName}: {EMessage}", nameof(T), e.Message);
            return [];
        }
    }

    protected virtual List<WorldPoco> GetWorlds() => [];

    protected abstract Task<List<int>> GetIdsToUpdateAsync(int? worldId, CancellationToken ct);
    protected virtual int GetApiSpamDelayInMs() => 60000;
}