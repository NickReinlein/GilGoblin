using System.Data;
using GilGoblin.Pocos;
using GilGoblin.Repository;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

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
        using var context = await GetContext();
        var itemInfoPoco = context?.ItemInfo?.FirstOrDefault(x => x.ID == itemID);
        return itemInfoPoco;
    }

    public async Task<IEnumerable<ItemInfoPoco>> GetAll()
    {
        using var context = await GetContext();
        return context?.ItemInfo?.ToList() ?? new List<ItemInfoPoco>();
    }

    public async Task<IEnumerable<ItemInfoPoco?>> GetMultiple(IEnumerable<int> itemIDs)
    {
        using var context = await GetContext();
        return context?.ItemInfo?.Where(i => itemIDs.Contains(i.ID)).ToList()
            ?? new List<ItemInfoPoco>();
    }

    private async Task<GilGoblinDbContext?> GetContext()
    {
        try
        {
            var connection = GoblinDatabase.Connect();
            connection?.Open();
            return await GoblinDatabase.GetContext();
        }
        catch (Exception e)
        {
            _logger.LogError("Unable to connect to database:", e.Message);
            return null;
        }
    }
}
