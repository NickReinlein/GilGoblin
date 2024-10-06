using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GilGoblin.Api.Repository;

public class PriceRepository(IServiceProvider serviceProvider, IPriceCache cache) : IPriceRepository<PricePoco>
{
    public PricePoco? Get(int worldId, int id, bool isHq)
    {
        var cached = cache.Get((worldId, id));
        if (cached is not null)
            return cached;

        try
        {
            using var scope = serviceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
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

    public List<PricePoco> GetMultiple(int worldId, IEnumerable<int> ids, bool isHq)
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        return dbContext.Price
            .Where(p =>
                p.WorldId == worldId &&
                p.IsHq == isHq &&
                ids.Any(i => i == p.ItemId))
            .ToList();
    }

    public List<PricePoco> GetAll(int worldId)
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        return dbContext.Price.Where(p => p.WorldId == worldId).ToList();
    }

    public Task FillCache()
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var items = dbContext?.Price?.ToList();
        items?.ForEach(price => cache.Add((price.WorldId, price.ItemId), price));
        return Task.CompletedTask;
    }
}