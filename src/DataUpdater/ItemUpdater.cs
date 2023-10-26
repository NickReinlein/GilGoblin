using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Fetcher;
using Microsoft.Extensions.Logging;

namespace GilGoblin.DataUpdater;

public class ItemUpdater : DataUpdater<ItemWebPoco>
{
    private readonly IItemFetcher _fetcher;

    public ItemUpdater(
        IItemFetcher fetcher,
        IDataSaver<ItemWebPoco> saver,
        ILogger<DataUpdater<ItemWebPoco>> logger)
        : base(fetcher, saver, logger)
    {
        _fetcher = fetcher;
    }

    protected override Task<List<List<int>>> GetIdsToUpdateAsync(int? worldId)
    {
        return Task.FromResult(new List<List<int>>());
    }
}