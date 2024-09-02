using System;
using System.Collections.Generic;
using System.Linq;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Pocos.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database.Savers;

public class PriceSaver : DataSaver<PricePoco>
{
    public PriceSaver(GilGoblinDbContext context, ILogger<DataSaver<PricePoco>> logger) : base(context, logger)
    {
    }

    protected override void UpdateContext(List<PricePoco> priceList)
    {
        var worldId = priceList.FirstOrDefault()?.WorldUploadTimes?.FirstOrDefault()?.WorldId ?? 0;
        if (worldId <= 0)
            throw new ArgumentException($"Missing or invalid world id {worldId} in price list");

        var itemIdList = priceList.Select(p => p.ItemId).ToList();
        var existing = Context.PriceDataPoints
            .Where(p =>
                p.WorldId == worldId &&
                itemIdList.Contains(p.ItemId))
            .Select(s => s.ItemId)
            .ToList();
        foreach (var price in priceList)
        {
            Context.Entry(price).State = existing.Contains(price.ItemId) ? EntityState.Modified : EntityState.Added;
        }
    }

    protected override List<PricePoco> FilterInvalidEntities(IEnumerable<PricePoco> entities)
    {
        return entities.Where(t =>
            t is { ItemId: > 0, WorldUploadTimes.Count: > 0 } &&
            t.WorldUploadTimes.Any(w => w is { WorldId: > 0, Timestamp: > 0 }) &&
            t.Hq.HasValidPrice() &&
            t.Nq.HasValidPrice()
        ).ToList();
    }
}