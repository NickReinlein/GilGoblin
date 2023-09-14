using GilGoblin.Database;
using GilGoblin.Pocos;
using GilGoblin.Web;
using Microsoft.Extensions.Logging;

namespace GilGoblin.DataUpdater;

public class ItemInfoUpdater : DataUpdater<ItemInfoWebPoco, ItemInfoWebResponse>
{
    public ItemInfoUpdater(
        GilGoblinDbContext dbContext,
        IDataFetcher<ItemInfoWebPoco,
        ItemInfoWebResponse> fetcher,
        ILogger<DataUpdater<ItemInfoWebPoco, ItemInfoWebResponse>> logger)
        : base(dbContext, fetcher, logger)
    {
    }


}