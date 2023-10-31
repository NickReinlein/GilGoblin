using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.Pocos;
using GilGoblin.Fetcher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.DataUpdater;

public class ItemUpdater : DataUpdater<ItemPoco, ItemWebPoco>
{
    public ItemUpdater(
        IServiceScopeFactory scopeFactory,
        ILogger<DataUpdater<ItemPoco, ItemWebPoco>> logger)
        : base(scopeFactory, logger)
    {
    }

    protected override async Task ConvertAndSaveToDbAsync(List<ItemWebPoco> updated)
    {
        using var scope = ScopeFactory.CreateScope();
        var saver = scope.ServiceProvider.GetRequiredService<IDataSaver<ItemPoco>>();
        var success = await saver.SaveAsync(updated.ToItemPocoList());
        if (!success)
            Logger.LogError($"Failed to save updates!");
    }

    protected override Task<List<int>> GetIdsToUpdateAsync(int? worldId)
    {
        return Task.FromResult(new List<int>());
    }
}