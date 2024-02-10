using System;
using System.Collections.Generic;
using System.Linq;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database
{
    public class PriceSaver : DataSaver<PricePoco>
    {
        public PriceSaver(GilGoblinDbContext context, ILogger<DataSaver<PricePoco>> logger) : base(context, logger)
        {
        }

        protected override void UpdateContext(List<PricePoco> priceList)
        {
            var worldId = priceList.First().WorldId;
            var itemList = priceList.Select(p => p.ItemId).ToList();
            var existing = Context.Price
                .Where(p =>
                    p.WorldId == worldId &&
                    itemList.Contains(p.ItemId))
                .Select(s => s.ItemId)
                .ToList();
            foreach (var price in priceList)
            {
                Context.Entry(price).State = existing.Contains(price.ItemId) ? EntityState.Modified : EntityState.Added;
            }
        }

        protected override void ValidateEntities(IEnumerable<PricePoco> entities)
        {
            if (entities.Any(t =>
                    t.WorldId <= 0 ||
                    t.ItemId <= 0 ||
                    t.LastUploadTime <= 0 ||
                    (t.AverageSold <= 0 &&
                     t.AverageListingPrice <= 0)
                ))
            {
                throw new ArgumentException("Cannot save entities due to error in key field");
            }
        }
    }
}