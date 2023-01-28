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

    public async Task<ItemInfoPoco?> Get(int itemID)
    {
        return await _database.GetItem(itemID);
    }

    public async Task<IEnumerable<ItemInfoPoco?>> GetAll()
    {
        var enumerable = Enumerable.Range(1, 5).Select(async (index) => await Get(index)).ToList();
        return null;
    }
}
