using System.Data;
using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Database;

public class ItemGateway : IItemRepository
{
    private readonly GoblinDatabase _database;
    private readonly ILogger<GoblinDatabase> _logger;

    public ItemGateway(GoblinDatabase database, ILogger<GoblinDatabase> logger)
    {
        _database = database;
        _logger = logger;
    }

    public async Task<ItemInfoPoco?> Get(int itemID)
    {
        _logger.LogInformation("Getting item {ID}", itemID);
        using var context = await GetContext();
        var itemInfoPoco = context?.ItemInfo?.FirstOrDefault(x => x.ID == itemID);
        return itemInfoPoco;
    }

    public async Task<IEnumerable<ItemInfoPoco>> GetAll()
    {
        _logger.LogInformation("Getting all items");
        using var context = await GetContext();
        return context?.ItemInfo?.ToList() ?? new List<ItemInfoPoco>();
    }

    public async Task<IEnumerable<ItemInfoPoco?>> GetMultiple(IEnumerable<int> itemIDs)
    {
        _logger.LogInformation("Getting {Number} items", itemIDs.Count());
        using var context = await GetContext();
        return context?.ItemInfo?.Where(i => itemIDs.Contains(i.ID)).ToList()
            ?? new List<ItemInfoPoco>();
    }

    private async Task<GilGoblinDbContext?> GetContext() => await _database.GetContextAsync();
}
