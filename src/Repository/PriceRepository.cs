using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Cache;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Repository;

public class PriceRepository : IPriceRepository<PricePoco>
{
    private readonly GilGoblinDbContext _dbContext;
    private readonly IPriceCache _cache;

    public PriceRepository(GilGoblinDbContext dbContext, IPriceCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public PricePoco? Get(int worldId, int id)
    {
        var cached = _cache.Get((worldId, id));
        if (cached is not null)
            return cached;

        try
        {
            var price = _dbContext.Price.First(p => p.WorldId == worldId && p.ItemId == id);
            _cache.Add((price.WorldId, price.ItemId), price);
            return price;
        }
        catch
        {
            return null;
        }
    }

    public IEnumerable<PricePoco> GetMultiple(int worldId, IEnumerable<int> ids) =>
        _dbContext.Price.Where(p => p.WorldId == worldId && ids.Any(i => i == p.ItemId));

    public IEnumerable<PricePoco> GetAll(int worldId) =>
        _dbContext.Price.Where(p => p.WorldId == worldId);

    public Task FillCache()
    {
        var items = _dbContext?.Price?.ToList();
        items?.ForEach(price => _cache.Add((price.WorldId, price.ItemId), price));
        return Task.CompletedTask;
    }
}