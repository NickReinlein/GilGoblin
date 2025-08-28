using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Savers;
using GilGoblin.Fetcher;
using GilGoblin.Fetcher.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GilGoblin.DataUpdater;

public class WorldUpdater(IServiceProvider serviceProvider, ILogger<WorldUpdater> logger)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await GetAllWorldsAsync();

            var delay = TimeSpan.FromMinutes(5);
            logger.LogInformation("{Service}: Awaiting delay of {Delay} seconds before performing next update",
                nameof(WorldUpdater), delay.TotalSeconds);
            await Task.Delay(delay, ct);
        }
    }

    public async Task GetAllWorldsAsync()
    {
        var result = await FetchAllWorldsAsync();
        await ConvertAndSaveToDbAsync(result);
    }

    private async Task<List<WorldWebPoco>> FetchAllWorldsAsync()
    {
        try
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var fetcher = scope.ServiceProvider.GetRequiredService<IWorldFetcher>();
            logger.LogInformation("Fetching updates for all worlds");
            var timer = new Stopwatch();
            timer.Start();
            var updated = await fetcher.GetAllAsync();
            timer.Stop();

            if (updated.Count == 0)
                logger.LogError("Received empty list returned when fetching all worlds");
            else
                logger.LogInformation("Received updates for {Count} worlds", updated.Count);

            logger.LogInformation("Total call time: {CallTime}ms", timer.Elapsed.TotalMilliseconds);

            //temporarily fetch less data while developing
            updated = new List<WorldWebPoco> { new (21, "Ravana"), new(22, "Bismarck"),  new(34, "Brynhildr") };
            ///// 
            return updated;
        }
        catch (Exception e)
        {
            logger.LogError("Failed to fetch updates for worlds: {Message}", e.Message);
            return [];
        }
    }

    private async Task ConvertAndSaveToDbAsync(List<WorldWebPoco> webPocos)
    {
        var updateList = webPocos.ToDatabasePoco();
        if (!updateList.Any())
            return;

        try
        {
            await using var scope = serviceProvider.CreateAsyncScope();
            var saver = scope.ServiceProvider.GetRequiredService<IDataSaver<WorldPoco>>();
            var success = await saver.SaveAsync(updateList);
            if (!success)
                throw new DbUpdateException($"Saving from {nameof(IDataSaver<WorldPoco>)} returned failure");
        }
        catch (Exception e)
        {
            logger.LogError("Failed to save {WebPocosCount} entries for {WorldPocoName}: {EMessage}", webPocos.Count, nameof(WorldPoco), e.Message);
        }
    }
}