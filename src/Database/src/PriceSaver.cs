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
        => updated.GetId() > 0 &&
           await DbContext.Price
               .AnyAsync(p =>
                   p.WorldId == updated.WorldId &&
                   p.ItemId == updated.ItemId);
}