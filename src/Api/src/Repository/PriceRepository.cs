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

public interface IPriceRepository<T> : IRepositoryCache where T : class
{
    T? Get(int worldId, int id, bool isHq);
    List<T> GetMultiple(int worldId, IEnumerable<int> ids, bool? isHq);
    List<T> GetAll(int worldId);
}

public class PriceRepository(IServiceProvider serviceProvider, IPriceCache cache) : IPriceRepository<PricePoco>
{
    public PricePoco? Get(int worldId, int id, bool isHq)
    {
        var cached = cache.Get(new TripleKey(worldId, id, isHq));
        if (cached is not null)
            return cached;

        try
        {
            using var scope = serviceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
            var price = dbContext.Price
                .Include(p => p.MinListing)
                .Include(p => p.RecentPurchase)
                .Include(p => p.AverageSalePrice)
                .Include(p => p.DailySaleVelocity)
                .FirstOrDefault(p =>
                    p.ItemId == id &&
                    p.WorldId == worldId &&
                    p.IsHq == isHq);
            if (price is not null)
                cache.Add(new TripleKey(worldId, id, isHq), price);

            return price;
        }
        catch
        {
            return null;
        }
    }

    public List<PricePoco> GetMultiple(int worldId, IEnumerable<int> ids, bool? isHq)
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        return dbContext.Price
            .Include(p => p.MinListing)
            .Include(p => p.RecentPurchase)
            .Include(p => p.AverageSalePrice)
            .Include(p => p.DailySaleVelocity)
            .Where(p =>
                p.WorldId == worldId &&
                (isHq == null || p.IsHq == isHq.Value) &&
                ids.Any(i => i == p.ItemId))
            .ToList();
    }

    public List<PricePoco> GetAll(int worldId)
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        return dbContext.Price
            .Include(p => p.MinListing)
            .Include(p => p.RecentPurchase)
            .Include(p => p.AverageSalePrice)
            .Include(p => p.DailySaleVelocity)
            .Where(p => p.WorldId == worldId).ToList();
    }

    public Task FillCache()
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var items = dbContext.Price.ToList();
        items.ForEach(price => cache.Add(new TripleKey(price.WorldId, price.ItemId, price.IsHq), price));
        return Task.CompletedTask;
    }
}