using GilGoblin.Pocos;

namespace GilGoblin.Repository;

public class CraftRepository : ICraftRepository<CraftSummaryPoco>
{
    public CraftSummaryPoco GetCraft(int worldId, int id)
    {
        return new CraftSummaryPoco
        {
            WorldID = worldId,
            ItemID = id,
            Name = "testItem" + id,
            AverageListingPrice = 300,
            AverageSold = 280,
            CraftingCost = 180,
            CraftingProfitVsListings = 300 - 180,
            CraftingProfitVsSold = 280 - 180,
            VendorPrice = 50
        };
    }

    public IEnumerable<CraftSummaryPoco> GetBestCrafts(int worldId)
    {
        return Enumerable.Range(1, 5).Select(index => GetCraft(worldId, index)).ToArray();
    }
}
