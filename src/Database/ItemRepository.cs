using System.Collections.Generic;
using System.Linq;
using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Database;

public class ItemRepository : IItemRepository
{
    private readonly GilGoblinDbContext _dbContext;

    public ItemRepository(GilGoblinDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public ItemInfoPoco? Get(int itemID) =>
        _dbContext?.ItemInfo?.FirstOrDefault(i => i.ID == itemID);

    public IEnumerable<ItemInfoPoco> GetMultiple(IEnumerable<int> itemIDs) =>
        _dbContext?.ItemInfo?.Where(i => itemIDs.Any(a => a == i.ID));

    public IEnumerable<ItemInfoPoco> GetAll() => _dbContext?.ItemInfo;
}
