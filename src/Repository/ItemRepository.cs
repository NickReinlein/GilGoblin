using GilGoblin.Database;
using GilGoblin.Pocos;

namespace GilGoblin.Repository;

public class ItemRepository : IItemRepository
{
    private readonly IItemGateway _database;
    private readonly ILogger<ItemRepository> _logger;

    public ItemRepository(IItemGateway database, ILogger<ItemRepository> logger)
    {
        _database = database;
        _logger = logger;
    }

    public ItemInfoPoco Get(int itemID)
    {
        return _database.GetItem(itemID);
        // return new ItemInfoPoco
        // {
        //     ID = itemID,
        //     Description = "TestItem" + itemID,
        //     GatheringID = itemID + 144,
        //     IconID = itemID + 42,
        //     Name = "TestItem" + itemID,
        //     StackSize = 1,
        //     VendorPrice = itemID
        // };
    }

    public IEnumerable<ItemInfoPoco> GetAll()
    {
        return Enumerable.Range(1, 5).Select(index => Get(index)).ToArray();
    }
}
