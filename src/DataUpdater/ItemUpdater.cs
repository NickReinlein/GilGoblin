using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.Pocos;
using GilGoblin.Fetcher;
using Microsoft.Extensions.Logging;

namespace GilGoblin.DataUpdater;

public class ItemUpdater : DataUpdater<ItemPoco, ItemWebPoco>
{
    private readonly IItemFetcher _fetcher;

    public ItemUpdater(
        IItemFetcher fetcher,
        IDataSaver<ItemPoco> saver,
        ILogger<DataUpdater<ItemPoco, ItemWebPoco>> logger)
        : base(saver, fetcher, logger)
    {
        _fetcher = fetcher;
    }

    protected override Task ConvertToDbFormatAndSave(List<ItemWebPoco> updated)
        => Saver.SaveAsync(updated.ToItemPocoList());

    protected override Task<List<int>> GetIdsToUpdateAsync(int? worldId)
    {
        return Task.FromResult(new List<int>());
    }
}