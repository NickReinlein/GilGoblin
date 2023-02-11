using GilGoblin.Pocos;
using GilGoblin.Repository;
using Serilog;

namespace GilGoblin.Database;

public class PriceGateway : IPriceRepository
{
    public async Task<PricePoco> Get(int worldID, int itemID)
    {
        return new PricePoco
        {
            ItemID = itemID,
            WorldID = worldID,
            AverageListingPrice = 100,
            AverageSold = 80,
        };
    }

    public async Task<IEnumerable<PricePoco?>> GetMultiple(int worldID, IEnumerable<int> itemIDs)
    {
        var returnList = new List<PricePoco?>();
        foreach (var itemId in itemIDs)
        {
            returnList.Add(await Get(worldID, itemId, GetWorldString()));
        }
        return returnList;
    }

    public async Task<IEnumerable<PricePoco>> GetAll(int worldID) =>
        await GetMultiple(worldID, Enumerable.Range(1, 20).ToArray());
}
