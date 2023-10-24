using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Fetcher;
using Microsoft.Extensions.Logging;

namespace GilGoblin.DataUpdater;

public class ItemUpdater : DataUpdater<ItemWebPoco, ItemWebResponse>
{
    private readonly IItemSingleFetcher _fetcher;

    public ItemUpdater(
        IItemSingleFetcher fetcher,
        IDataSaver<ItemWebPoco> saver,
        ILogger<DataUpdater<ItemWebPoco, ItemWebResponse>> logger)
        : base(fetcher, saver, logger)
    {
        _fetcher = fetcher;
    }

    protected override async Task<List<List<ItemWebPoco>>> GetEntriesToUpdateAsync()
    {
        var idBatches = await _fetcher.GetIdsAsBatchJobsAsync();

        return idBatches.Select(batch =>
            batch.Select(id => new ItemWebPoco { Id = id })
                .ToList()
        ).ToList();
    }
}