using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Cache;
using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Database;

public class ItemRepository : IItemRepository
{
    private readonly GilGoblinDbContext _dbContext;
    private readonly IItemInfoCache _cache;

    public ItemRepository(GilGoblinDbContext dbContext, IItemInfoCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public ItemInfoPoco? Get(int itemID)
    {
        var cached = _cache.Get(itemID);
        if (cached is not null)
            return cached;

        var item = _dbContext?.ItemInfo?.FirstOrDefault(i => i.ID == itemID);
        if (item is not null)
            _cache.Add(item.ID, item);

        return item;
    }

    public IEnumerable<ItemInfoPoco> GetMultiple(IEnumerable<int> itemIDs) =>
        _dbContext?.ItemInfo?.Where(i => itemIDs.Any(a => a == i.ID));

    public IEnumerable<ItemInfoPoco> GetAll() => _dbContext?.ItemInfo;

    public async Task FillCache()
    {
        var items = await _dbContext?.ItemInfo?.ToListAsync();
        items.ForEach(item => _cache.Add(item.ID, item));
    }
}
