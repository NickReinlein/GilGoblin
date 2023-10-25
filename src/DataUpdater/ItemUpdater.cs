using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Fetcher;
using Microsoft.Extensions.Logging;

namespace GilGoblin.DataUpdater;

public class ItemUpdater : DataUpdater<ItemWebPoco>
{
    private readonly IItemSingleFetcher _fetcher;

    public ItemUpdater(
        IItemSingleFetcher fetcher,
        IDataSaver<ItemWebPoco> saver,
        ILogger<DataUpdater<ItemWebPoco>> logger)
        : base(fetcher, saver, logger)
    {
        _fetcher = fetcher;
    }

    protected override async Task<List<int>> GetIdsToUpdateAsync()
        => await _fetcher.GetAllMissingIds();
}