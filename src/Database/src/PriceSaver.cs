using System.Collections.Generic;
using System.Linq;
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

    protected override async Task<bool> ShouldBeUpdated(PricePoco updated)
        => await DbContext.Price
            .AnyAsync(p =>
                p.WorldId == updated.WorldId &&
                p.ItemId == updated.ItemId &&
                p.LastUploadTime < updated.LastUploadTime);

    public override bool SanityCheck(IEnumerable<PricePoco> updates)
        => true;
    // => !updates.Any(price => price.WorldId <= 0 ||
    //                          price.ItemId <= 0 ||
    //                          price.LastUploadTime <= 0);
}