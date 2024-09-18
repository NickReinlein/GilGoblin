using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Api.Cache;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Api.Repository;

public class ItemRepository(IServiceProvider serviceProvider, IItemCache cache, ILogger<ItemRepository> logger)
    : IItemRepository
{
    public ItemPoco? Get(int itemId)
    {
        var cached = cache.Get(itemId);
        if (cached is not null)
            return cached;

        try
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
            var item = dbContext.Item.FirstOrDefault(i => i.Id == itemId);
            if (item is null)
                return null;

            cache.Add(item.Id, item);
            return item;
        }
        catch (Exception e)
        {
            logger.LogWarning("Failed to get item {ItemId}: {Message}", itemId, e.Message);
            return null;
        }
    }

    public IEnumerable<ItemPoco> GetMultiple(IEnumerable<int> itemIds)
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        return dbContext.Item.Where(i => itemIds.Any(a => a == i.Id)).AsEnumerable();
    }

    public IEnumerable<ItemPoco> GetAll()
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        return dbContext.Item.AsEnumerable();
    }

    public Task FillCache()
    {
        using var scope = serviceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var items = dbContext.Item.ToList();
        items.ForEach(item => cache.Add(item.Id, item));
        return Task.CompletedTask;
    }
}