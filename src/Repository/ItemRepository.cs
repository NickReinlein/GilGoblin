using GilGoblin.Pocos;

namespace GilGoblin.Repository;

public class ItemRepository : IItemRepository
{
    public ItemInfoPoco Get(int id)
    {
        return new ItemInfoPoco
        {
            ID = id,
            Description = "TestItem" + id,
            GatheringID = id + 144,
            IconID = id + 42,
            Name = "TestItem" + id,
            StackSize = 1,
            VendorPrice = id
        };
    }

    public IEnumerable<ItemInfoPoco> GetAll()
    {
        return Enumerable.Range(1, 5).Select(index => Get(index)).ToArray();
    }
}
