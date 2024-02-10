using System.Collections.Generic;
using System.Linq;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database;

public class PriceSaver : DataSaver<PricePoco>, IPriceSaver
{
    public PriceSaver(GilGoblinDbContext context,
        ILogger<DataSaver<PricePoco>> logger)
        : base(context, logger)
    {
    }

    public override bool SanityCheck(IEnumerable<PricePoco> updates)
    {
        var pocoList = updates.ToList();
        return !pocoList
            .Any(price =>
                price.WorldId <= 0 ||
                price.ItemId <= 0 ||
                price.LastUploadTime <= 1704114061); // older than 2024-01-01
    }
}

public interface IPriceSaver : IDataSaver<PricePoco>
{
}