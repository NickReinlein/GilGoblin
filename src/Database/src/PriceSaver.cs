using System.Collections.Generic;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database;

public class PriceSaver : DataSaver<PricePoco>
{
    public PriceSaver(GilGoblinDbContext dbContext,
        ILogger<DataSaver<PricePoco>> logger)
        : base(dbContext, logger)
    {
    }

    protected override async Task UpdatedExistingEntries(List<PricePoco> updatedEntries)
    {
        foreach (var updated in updatedEntries)
        {
            var existingEntity
                = await DbContext.Price
                    .FirstOrDefaultAsync(p =>
                        p.WorldId == updated.WorldId &&
                        p.ItemId == updated.ItemId);
            if (existingEntity != null)
                DbContext.Entry(existingEntity).CurrentValues.SetValues(updated);
        }
    }
}