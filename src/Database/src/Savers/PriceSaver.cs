using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Savers;

public class PriceSaver(IServiceProvider serviceProvider, ILogger<DataSaver<PricePoco>> logger)
    : DataSaver<PricePoco>(serviceProvider, logger)
{
    protected override async Task<int> UpdateContextAsync(List<PricePoco> priceList)
    {
        await using var scope = ServiceProvider.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<GilGoblinDbContext>();
        var existing = dbContext.Price
            .Where(p => priceList.Any(l =>
                p.WorldId == l.WorldId &&
                p.ItemId == l.ItemId &&
                p.IsHq == l.IsHq))
            .Select(s => s.ItemId)
            .ToList();
        foreach (var price in priceList)
        {
            dbContext.Entry(price).State = existing.Contains(price.ItemId) ? EntityState.Modified : EntityState.Added;
        }
        return await dbContext.SaveChangesAsync();
    }

    protected override List<PricePoco> FilterInvalidEntities(IEnumerable<PricePoco> entities)
    {
        return entities.Where(t => t is { ItemId: > 0, WorldId : > 0 }).ToList();
    }
}