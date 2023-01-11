using GilGoblin.Pocos;

namespace GilGoblin.Repository;

public class PriceRepository : IPriceRepository
{
    public PricePoco Get(int worldID, int itemID)
    {
        return new PricePoco
        {
            ItemID = itemID,
            WorldID = worldID,
            LastUploadTime = 10,
            Name = "testObject" + itemID,
            RegionName = "MountFuji",
            AverageListingPrice = itemID * 5f,
            AverageListingPriceNQ = itemID * 3f,
            AverageListingPriceHQ = itemID * 8f,
            AverageSold = itemID * 6f,
            AverageSoldNQ = itemID * 5f,
            AverageSoldHQ = itemID * 9f,
        };
    }

    public IEnumerable<PricePoco> GetAll(int worldID)
    {
        return Enumerable.Range(1, 5).Select(index => Get(worldID, index)).ToArray();
    }
}
