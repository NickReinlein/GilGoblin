using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Savers;
using GilGoblin.Fetcher.Pocos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.DataUpdater;

// Works but currently unused and untested
public class ItemUpdater(
    IServiceProvider serviceProvider,
    ILogger<DataUpdater<ItemPoco, ItemWebPoco>> logger)
    : DataUpdater<ItemPoco, ItemWebPoco>(serviceProvider, logger)
{
    protected override async Task ExecuteUpdateAsync(CancellationToken ct)
    {
        var worlds = GetWorlds();
        var worldIdString = !worlds.Any() ? string.Empty : $" for world {worlds}";
        Logger.LogInformation("Fetching updates of type {ItemPocoName}{WorldIdString}", nameof(ItemPoco), worldIdString);
        await FetchAsync(worlds.FirstOrDefault()?.GetId(), ct);
    }

    protected override async Task ConvertAndSaveToDbAsync(List<ItemWebPoco> updated, int? worldId = null,
        CancellationToken ct = default)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var saver = scope.ServiceProvider.GetRequiredService<IDataSaver<ItemPoco>>();
        var success = await saver.SaveAsync(updated.ToItemPocoList(), ct);
        if (!success)
            Logger.LogError("Failed to save updates!");
    }

    protected override Task<List<int>> GetIdsToUpdateAsync(int? worldId, CancellationToken ct)
    {
        return Task.FromResult(new List<int>());
    }
}