using System;
using System.Collections.Generic;
using System.Linq;
using GilGoblin.Database.Pocos;
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
        var worldId = priceList.FirstOrDefault()?.WorldId;
        if (worldId is null or <= 0)
            throw new ArgumentException($"Missing or invalid world id {worldId}");

        var itemIdList = priceList.Select(p => p.ItemId).ToList();
        var existing = Context.Price
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
            t is { WorldId: >= 0, ItemId: >= 0, LastUploadTime: >= 0 } &&
            (t.AverageSold >= 0 || t.AverageListingPrice >= 0)
        ).ToList();
    }
}