using GilGoblin.Pocos;
using Serilog;

namespace GilGoblin.Database;

public class PriceGateway : IPriceGateway
{
    public PricePoco GetPrice(int worldID, int itemID)
    {
        return new PricePoco
        {
            ItemID = itemID,
            WorldID = worldID,
            AverageListingPrice = 100,
            AverageSold = 80,
            Name = "TestItem",
            RegionName = "TestRealm"
        };
    }

    public IEnumerable<PricePoco> GetPrices(int worldID, IEnumerable<int> itemIDs)
    {
        foreach (var itemId in itemIDs)
        {
            yield return GetPrice(worldID, itemId);
        }
    }

    public IEnumerable<PricePoco> GetAllPrices(int worldID) =>
        GetPrices(worldID, Enumerable.Range(1, 20).ToArray());
}
