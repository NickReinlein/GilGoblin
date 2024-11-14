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

public interface IItemRepository : IDataRepository<ItemPoco>;

public class ItemRepository(
    IServiceProvider serviceProvider,
    IItemCache cache,
    ILogger<ItemRepository> logger)
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
            using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
            var item = dbContext.Item.FirstOrDefault(i => i.Id == itemId);
            if (item is null)
                return null;

            cache.Add(item.GetId(), item);
            return item;
        }
        catch (Exception e)
        {
            logger.LogWarning("Failed to get item {ItemId}: {Message}", itemId, e.Message);
            return null;
        }
    }

    public List<ItemPoco> GetMultiple(IEnumerable<int> ids)
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        return dbContext.Item.Where(i => ids.Any(a => a == i.Id)).ToList();
    }

    public List<ItemPoco> GetAll()
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        return dbContext.Item.ToList();
    }

    public Task FillCache()
    {
        using var scope = serviceProvider.CreateScope();
        using var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var items = dbContext.Item.ToList();
        items.ForEach(item => cache.Add(item.GetId(), item));
        return Task.CompletedTask;
    }
}