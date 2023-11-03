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

    public ItemPoco? Get(int recipeId)
    {
        var cached = _cache.Get(recipeId);
        if (cached is not null)
            return cached;

        var item = _dbContext?.Item?.FirstOrDefault(i => i.Id == recipeId);
        if (item is not null)
            _cache.Add(item.Id, item);

        return item;
    }

    public IEnumerable<ItemPoco> GetMultiple(IEnumerable<int> recipeIds) =>
        _dbContext?.Item?.Where(i => recipeIds.Any(a => a == i.Id));

    public IEnumerable<ItemPoco> GetAll() => _dbContext?.Item;

    public Task FillCache()
    {
        var items = _dbContext?.Item?.ToList();
        items?.ForEach(item => _cache.Add(item.Id, item));
        return Task.CompletedTask;
    }
}