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

    public ItemInfoPoco? Get(int recipeId)
    {
        var cached = _cache.Get(recipeId);
        if (cached is not null)
            return cached;

        var item = _dbContext?.ItemInfo?.FirstOrDefault(i => i.Id == recipeId);
        if (item is not null)
            _cache.Add(item.Id, item);

        return item;
    }

    public IEnumerable<ItemInfoPoco> GetMultiple(IEnumerable<int> recipeIds) =>
        _dbContext?.ItemInfo?.Where(i => recipeIds.Any(a => a == i.Id));

    public IEnumerable<ItemInfoPoco> GetAll() => _dbContext?.ItemInfo;

    public Task FillCache()
    {
        var items = _dbContext?.ItemInfo?.ToList();
        items.ForEach(item => _cache.Add(item.Id, item));
        return Task.CompletedTask;
    }
}
