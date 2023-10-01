using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GilGoblin.DataUpdater;

public class ItemInfoUpdater : DataUpdater<ItemInfoWebPoco, ItemInfoWebResponse>
{
    protected new readonly IItemInfoFetcher Fetcher;

    public ItemInfoUpdater(
        IItemInfoFetcher fetcher,
        IDataSaver<ItemInfoWebPoco> saver,
        ILogger<DataUpdater<ItemInfoWebPoco, ItemInfoWebResponse>> logger)
        : base(fetcher, saver, logger)
    {
        Fetcher = fetcher;
    }
    //
    // protected override async Task<List<ItemInfoWebPoco>> FetchUpdateAsync(
    //     IEnumerable<ItemInfoWebPoco> entriesToUpdate)
    // {
    //     var idsToUpdate = await GetIdsToUpdateAsync();
    //     if (!idsToUpdate.Any())
    //     {
    //         _logger.LogInformation($"No entries need to be updated for {nameof(ItemInfoWebPoco)}");
    //         return new List<ItemInfoWebPoco>();
    //     }
    //
    //     _logger.LogInformation($"Fetching updates for {idsToUpdate.Count()} {nameof(ItemInfoWebPoco)} entries");
    //     return await FetchUpdatesForIDsAsync(idsToUpdate);
    // }
    
    // private async Task<List<ItemInfoWebPoco>> FetchUpdatesForIDsAsync(IEnumerable<int> idsToUpdate)
    // {
    //     try
    //     {
    //         var timer = new Stopwatch();
    //         timer.Start();
    //         var updated = await Fetcher.FetchByIdsAsync(idsToUpdate);
    //         timer.Stop();
    //         var callTime = timer.Elapsed.TotalMilliseconds;
    //
    //         _logger.LogInformation($"Received updates for {updated.Count} {nameof(ItemInfoWebPoco)} entries");
    //         _logger.LogInformation($"Total call time: {callTime}");
    //         return updated;
    //     }
    //     catch (Exception e)
    //     {
    //         _logger.LogError($"Failed to fetch updates for {nameof(ItemInfoWebPoco)}: {e.Message}");
    //         return new List<ItemInfoWebPoco>();
    //     }
    // }
}