using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Api.Repository;

public class PriceRepository(GilGoblinDbContext dbContext, IPriceCache cache) : IPriceRepository<PricePoco>
{
    public PricePoco? Get(int worldId, int id, bool isHq)
    {
        var cached = cache.Get((worldId, id));
        if (cached is not null)
            return cached;

        try
        {
            var prices = dbContext.Price
                .Where(p =>
                    p.ItemId == id &&
                    p.WorldId == worldId &&
                    p.IsHq == isHq)
                .ToList();

            if (!prices.Any())
                throw new Exception($"No prices found for item id {id}");

            var price = prices.FirstOrDefault(p => p.WorldId == worldId);

            if (price is not null)
                cache.Add((worldId, id), price);

            return price;
        }
        catch
        {
            return null;
        }
    }

    public IEnumerable<PricePoco> GetMultiple(int worldId, IEnumerable<int> ids, bool isHq) =>
        dbContext.Price.Where(p =>
            p.WorldId == worldId
            && ids.Any(i => i == p.ItemId)
            && p.IsHq == isHq);

    public IEnumerable<PricePoco> GetAll(int worldId)
    {
        var allPrices = dbContext.Price.Where(p => p.WorldId == worldId);
        return allPrices;
    }

    public Task FillCache()
    {
        var items = dbContext?.Price?.ToList();
        items?.ForEach(price => cache.Add((price.WorldId, price.ItemId), price));
        return Task.CompletedTask;
    }
}