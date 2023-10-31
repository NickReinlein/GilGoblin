using System.Collections.Generic;
using System.Linq;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Fetcher;

public static class PriceWebPocoExtensions
{
    public static PricePoco ToPricePoco(this PriceWebPoco webPoco) =>
        new()
        {
            ItemId = webPoco.ItemId,
            WorldId = webPoco.WorldId,
            LastUploadTime = webPoco.LastUploadTime,
            AverageListingPrice = webPoco.CurrentAveragePrice,
            AverageListingPriceNQ = webPoco.CurrentAveragePriceNQ,
            AverageListingPriceHQ = webPoco.CurrentAveragePriceHQ,
            AverageSold = webPoco.AveragePrice,
            AverageSoldNQ = webPoco.AveragePriceNQ,
            AverageSoldHQ = webPoco.AveragePriceHQ
        };

    public static List<PricePoco> ToPricePocoList(this IEnumerable<PriceWebPoco?> pocos) =>
        pocos.Where(poco => poco is not null).Select(price => price.ToPricePoco()).ToList();
}