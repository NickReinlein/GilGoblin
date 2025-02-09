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
    T? Get(int worldId, int itemId, bool isHq);
    List<T> GetMultiple(int worldId, IEnumerable<int> itemIds, bool? isHq = null);
    List<T> GetAll(int worldId);
}

public class PriceRepository(IServiceProvider serviceProvider, IPriceCache cache) : IPriceRepository<PricePoco>
{
    public PricePoco? Get(int worldId, int itemId, bool isHq)
    {
        var cached = cache.Get(new TripleKey(worldId, itemId, isHq));
        if (cached is not null)
            return cached;

        try
        {
            using var scope = serviceProvider.CreateScope();
            using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
            var price = dbContext.Price
                .AsNoTracking()
                .Include(p => p.MinListing)
                .ThenInclude(p => p!.DcDataPoint)
                .Include(p => p.MinListing)
                .ThenInclude(p => p!.RegionDataPoint)
                .Include(p => p.MinListing)
                .ThenInclude(p => p!.WorldDataPoint)
                .Include(p => p.RecentPurchase)
                .ThenInclude(p => p!.DcDataPoint)
                .Include(p => p.RecentPurchase)
                .ThenInclude(p => p!.RegionDataPoint)
                .Include(p => p.RecentPurchase)
                .ThenInclude(p => p!.WorldDataPoint)
                .Include(p => p.AverageSalePrice)
                .ThenInclude(p => p!.DcDataPoint)
                .Include(p => p.AverageSalePrice)
                .ThenInclude(p => p!.RegionDataPoint)
                .Include(p => p.AverageSalePrice)
                .ThenInclude(p => p!.WorldDataPoint)
                .Include(p => p.DailySaleVelocity)
                .FirstOrDefault(p =>
                    p.ItemId == itemId &&
                    p.WorldId == worldId &&
                    p.IsHq == isHq);
            if (price is not null)
                cache.Add(new TripleKey(worldId, itemId, isHq), price);

            return price;
        }
        catch
        {
            return null;
        }
    }

    public List<PricePoco> GetMultiple(int worldId, IEnumerable<int> itemIds, bool? isHq)
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        return dbContext.Price
            .AsNoTracking()
            .Include(p => p.MinListing)
            .ThenInclude(p => p!.DcDataPoint)
            .Include(p => p.MinListing)
            .ThenInclude(p => p!.RegionDataPoint)
            .Include(p => p.MinListing)
            .ThenInclude(p => p!.WorldDataPoint)
            .Include(p => p.RecentPurchase)
            .ThenInclude(p => p!.DcDataPoint)
            .Include(p => p.RecentPurchase)
            .ThenInclude(p => p!.RegionDataPoint)
            .Include(p => p.RecentPurchase)
            .ThenInclude(p => p!.WorldDataPoint)
            .Include(p => p.AverageSalePrice)
            .ThenInclude(p => p!.DcDataPoint)
            .Include(p => p.AverageSalePrice)
            .ThenInclude(p => p!.RegionDataPoint)
            .Include(p => p.AverageSalePrice)
            .ThenInclude(p => p!.WorldDataPoint)
            .Include(p => p.DailySaleVelocity)
            .Where(p =>
                p.WorldId == worldId &&
                (isHq == null || p.IsHq == isHq.Value) &&
                itemIds.Any(i => i == p.ItemId))
            .ToList();
    }

    public List<PricePoco> GetAll(int worldId)
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        return dbContext.Price
            .AsNoTracking()
            .Include(p => p.MinListing)
            .ThenInclude(p => p!.DcDataPoint)
            .Include(p => p.MinListing)
            .ThenInclude(p => p!.RegionDataPoint)
            .Include(p => p.MinListing)
            .ThenInclude(p => p!.WorldDataPoint)
            .Include(p => p.RecentPurchase)
            .ThenInclude(p => p!.DcDataPoint)
            .Include(p => p.RecentPurchase)
            .ThenInclude(p => p!.RegionDataPoint)
            .Include(p => p.RecentPurchase)
            .ThenInclude(p => p!.WorldDataPoint)
            .Include(p => p.AverageSalePrice)
            .ThenInclude(p => p!.DcDataPoint)
            .Include(p => p.AverageSalePrice)
            .ThenInclude(p => p!.RegionDataPoint)
            .Include(p => p.AverageSalePrice)
            .ThenInclude(p => p!.WorldDataPoint)
            .Include(p => p.DailySaleVelocity)
            .Where(p => p.WorldId == worldId)
            .ToList();
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