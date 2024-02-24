using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Repository;

public class ItemRepository : IItemRepository
{
    private readonly GilGoblinDbContext _dbContext;
    private readonly IItemCache _cache;

    public ItemRepository(GilGoblinDbContext dbContext, IItemCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public ItemPoco? Get(int itemId)
    {
        var cached = _cache.Get(itemId);
        if (cached is not null)
            return cached;

        var item = _dbContext?.Item?.FirstOrDefault(i => i.Id == itemId);
        if (item is not null)
            _cache.Add(item.Id, item);

        return item;
    }

    public IEnumerable<ItemPoco> GetMultiple(IEnumerable<int> itemIds) =>
        _dbContext?.Item?.Where(i => itemIds.Any(a => a == i.Id)).AsEnumerable();

    public IEnumerable<ItemPoco> GetAll() => _dbContext?.Item.AsEnumerable();

    public Task FillCache()
    {
        var items = _dbContext?.Item?.ToList();
        items?.ForEach(item => _cache.Add(item.Id, item));
        return Task.CompletedTask;
    }
}