using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Web;
using Microsoft.Extensions.Logging;

namespace GilGoblin.DataUpdater;

public class ItemInfoUpdater : DataUpdater<ItemInfoWebPoco, ItemInfoWebResponse>
{
    protected new IItemInfoFetcher _fetcher;
    public ItemInfoUpdater(GilGoblinDbContext dbContext, IItemInfoFetcher fetcher, ILogger<DataUpdater<ItemInfoWebPoco, ItemInfoWebResponse>> logger) : base(dbContext, fetcher, logger)
    {
        _fetcher = fetcher;
    }

    protected override async Task<IEnumerable<ItemInfoWebPoco>> FetchUpdateAsync()
    {

        var idsToUpdate = await GetIdsToUpdateAsync();
        if (!idsToUpdate.Any())
        {
            _logger.LogInformation($"No entries need to be updated for {nameof(ItemInfoWebPoco)}");
            return Enumerable.Empty<ItemInfoWebPoco>();
        }

        _logger.LogInformation($"Fetching updates for {idsToUpdate.Count()} {nameof(ItemInfoWebPoco)} entries");
        return await FetchUpdatesForIDs(idsToUpdate);
    }

    protected async Task<IEnumerable<int>> GetIdsToUpdateAsync()
    {
        var allIDs = await _fetcher.GetMarketableItemIDsAsync();
        if (!allIDs.Any())
            return Enumerable.Empty<int>();

        var existingIDs = await _dbContext?.ItemInfo?.Select(i => i.ID).ToListAsync();

        return allIDs.Except(existingIDs);
    }

    private async Task<IEnumerable<ItemInfoWebPoco>> FetchUpdatesForIDs(IEnumerable<int> idsToUpdate)
    {
        var timer = new Stopwatch();
        timer.Start();
        var updated = await _fetcher.FetchMultipleItemInfosAsync(idsToUpdate);
        timer.Stop();
        var callTime = timer.Elapsed.TotalMilliseconds;

        _logger.LogInformation($"Received updates for {updated.Count()} {nameof(ItemInfoWebPoco)} entries in {callTime}ms");
        return updated;
    }
}