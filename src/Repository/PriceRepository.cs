using GilGoblin.Pocos;

namespace GilGoblin.Repository;

public class PriceRepository : IPriceRepository
{
    public MarketDataPoco Get(int id)
    {
        return new MarketDataPoco
        {
            ItemID = id,
            WorldID = 42,
            LastUploadTime = 10,
            Name = "testObject" + id,
            RegionName = "MountFuji",
            AverageListingPrice = id * 5f,
            AverageListingPriceNQ = id * 3f,
            AverageListingPriceHQ = id * 8f,
            AverageSold = id * 6f,
            AverageSoldNQ = id * 5f,
            AverageSoldHQ = id * 9f,
        };
    }

    public IEnumerable<MarketDataPoco> GetAll()
    {
        return Enumerable.Range(1, 5).Select(index => Get(index)).ToArray();
    }
}