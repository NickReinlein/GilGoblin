using System.Collections.Generic;
using System.Linq;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Savers;

public class PriceSaver(GilGoblinDbContext context, ILogger<DataSaver<PricePoco>> logger)
    : DataSaver<PricePoco>(context, logger)
{
    protected override void UpdateContext(List<PricePoco> priceList)
    {
        var existing = Context.Price
            .Where(p => priceList.Any(l =>
                p.WorldId == l.WorldId &&
                p.ItemId == l.ItemId &&
                p.IsHq == l.IsHq))
            .Select(s => s.ItemId)
            .ToList();
        foreach (var price in priceList)
        {
            Context.Entry(price).State = existing.Contains(price.ItemId) ? EntityState.Modified : EntityState.Added;
        }
    }

    protected override List<PricePoco> FilterInvalidEntities(IEnumerable<PricePoco> entities)
    {
        return entities.Where(t => t is { ItemId: > 0, WorldId : > 0 }).ToList();
    }
}