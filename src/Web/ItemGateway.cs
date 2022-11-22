using GilGoblin.Pocos;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Web;

public class ItemGateway : IItemGateway
{
    public ItemInfoPoco GetItem(int itemID)
    {
        return new ItemInfoPoco()
        {
            ID = itemID,
            Name = "testName",
            Description = "testDescription",
            GatheringID = 999,
            IconID = 3200,
            StackSize = 20,
            VendorPrice = new Random().Next(int.MinValue, int.MaxValue)
        };
    }

    public IEnumerable<ItemInfoPoco> GetItems(IEnumerable<int> itemIDs)
    {
        foreach (var itemId in itemIDs)
        {
            yield return GetItem(itemId);
        }
    }
    public IEnumerable<ItemInfoPoco> GetAllItems() => GetItems(Enumerable.Range(1, 100).ToArray());
}