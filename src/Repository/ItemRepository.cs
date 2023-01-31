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

    public ItemInfoPoco? Get(int itemID)
    {
        return _database.GetItem(itemID);
    }

    public IEnumerable<ItemInfoPoco?> GetAll()
    {
        // var enumerable = Enumerable.Range(1, 5).Select((index) => Get(index)).ToList();
        return Enumerable.Range(1, 5).Select(Get).ToList();
    }
}
