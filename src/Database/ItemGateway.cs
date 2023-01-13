using GilGoblin.Pocos;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database;

public class ItemGateway : IItemGateway
{
    private readonly GoblinDatabase _database;
    private readonly ILogger<GoblinDatabase> _logger;

    public ItemGateway(GoblinDatabase database, ILogger<GoblinDatabase> logger)
    {
        _database = database;
        _logger = logger;
    }

    public ItemInfoPoco GetItem(int itemID)
    {
        try
        {
            var connection = GoblinDatabase.Connect();
        }
        catch (Exception e)
        {
            _logger.LogError("Unable to connect to database:", e.Message);
        }
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

    public IEnumerable<ItemInfoPoco> GetAllItems() => GetItems(Enumerable.Range(1, 10).ToArray());
}
