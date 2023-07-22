using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database;

public class ItemGateway : IItemRepository
{
    private readonly GilGoblinDatabase _database;
    private readonly ILogger<GilGoblinDatabase> _logger;

    public ItemGateway(GilGoblinDatabase database, ILogger<GilGoblinDatabase> logger)
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
