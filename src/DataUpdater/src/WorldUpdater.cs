using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.Fetcher;
using GilGoblin.Fetcher.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GilGoblin.DataUpdater;

public class WorldUpdater(IWorldFetcher Fetcher, IDataSaver<WorldPoco> Saver, ILogger<WorldUpdater> Logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                await GetAllWorldsAsync();
            }
            catch (Exception ex)
            {
                Logger.LogError("{Service}: An exception occured during the Api call: {ErrorMessage}",
                    nameof(WorldUpdater), ex.Message);
            }

            var delay = TimeSpan.FromMinutes(5);
            Logger.LogInformation("{Service}: Awaiting delay of {Delay} seconds before performing next update",
                nameof(WorldUpdater), delay.TotalSeconds);
            await Task.Delay(delay, ct);
        }
    }

    public async Task GetAllWorldsAsync()
    {
        var result = await FetchAllWorlds();
        await ConvertAndSaveToDbAsync(result);
    }

    private async Task<List<WorldWebPoco>> FetchAllWorlds()
    {
        try
        {
            Logger.LogInformation("Fetching updates for all worlds");
            var timer = new Stopwatch();
            timer.Start();
            var updated = await Fetcher.GetAllAsync();
            timer.Stop();

            if (updated.Count == 0)
                Logger.LogError("Received empty list returned when fetching all worlds");
            else
                Logger.LogInformation("Received updates for {Count} worlds", updated.Count);

            Logger.LogInformation("Total call time: {CallTime}ms", timer.Elapsed.TotalMilliseconds);
            return updated;
        }
        catch (Exception e)
        {
            Logger.LogError("Failed to fetch updates for worlds: {Message}", e.Message);
            return [];
        }
    }

    private async Task ConvertAndSaveToDbAsync(List<WorldWebPoco> webPocos)
    {
        var updateList = webPocos.ToWorldPocoList();
        if (!updateList.Any())
            return;

        try
        {
            var success = await Saver.SaveAsync(updateList);
            if (!success)
                throw new DbUpdateException($"Saving from {nameof(IDataSaver<WorldPoco>)} returned failure");
        }
        catch (Exception e)
        {
            Logger.LogError($"Failed to save {webPocos.Count} entries for {nameof(WorldPoco)}: {e.Message}");
        }
    }
}