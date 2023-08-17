using System.Collections.Generic;
using System.Linq;
using GilGoblin.Cache;
using GilGoblin.Pocos;
using GilGoblin.Repository;

namespace GilGoblin.Database;

public class PriceRepository : IPriceRepository<PricePoco>
{
    private readonly GilGoblinDbContext _dbContext;
    private readonly IPriceCache _cache;

    public PriceRepository(GilGoblinDbContext dbContext, IPriceCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public PricePoco? Get(int worldID, int id) =>
        _dbContext.Price.FirstOrDefault(p => p.WorldID == worldID && p.ItemID == id);

    public IEnumerable<PricePoco> GetMultiple(int worldID, IEnumerable<int> ids) =>
        _dbContext.Price.Where(p => p.WorldID == worldID && ids.Any(i => i == p.ItemID));

    public IEnumerable<PricePoco> GetAll(int worldID) =>
        _dbContext.Price.Where(p => p.WorldID == worldID);
}
