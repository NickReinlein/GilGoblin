using System.Data;
using GilGoblin.Pocos;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database;

public class ItemGateway : IItemGateway
{
    private readonly GoblinDatabase _database;
    private readonly GilGoblinDbContext _context;
    private readonly ILogger<GoblinDatabase> _logger;

    public ItemGateway(
        GoblinDatabase database,
        ILogger<GoblinDatabase> logger,
        GilGoblinDbContext context
    )
    {
        _database = database;
        _logger = logger;
        _context = context;
    }

    public async Task<ItemInfoPoco?> GetItem(int itemID)
    {
        try
        {
            using var connection = GoblinDatabase.Connect();
            connection.Open();
            return _context.ItemInfo?.Single(x => x.ID == itemID);
        }
        catch (Exception e)
        {
            _logger.LogError("Unable to connect to database:", e.Message);
            return null;
        }
    }

    public async Task<IEnumerable<ItemInfoPoco>> GetItems(IEnumerable<int> itemIDs)
    {
        var items = new List<ItemInfoPoco>();
        foreach (var itemId in itemIDs)
        {
            items.Add(await GetItem(itemId));
        }
        return items;
    }

    public async Task<IEnumerable<ItemInfoPoco>> GetAllItems() =>
        await GetItems(Enumerable.Range(1, 10).ToArray());
}
