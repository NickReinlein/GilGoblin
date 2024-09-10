using System.Collections.Generic;

namespace GilGoblin.Database.Pocos.Extensions;

public static class QualityPriceDataPocoExtensions
{
    public static bool HasValidPrice(this QualityPriceDataPoco? poco) =>
        poco is not null &&
        (poco.MinListing.HasValidPrice() || 
        poco.AverageSalePrice.HasValidPrice() || 
        poco.RecentPurchase.HasValidPrice());

    public static List<PriceDataDbPoco> ToQualityPriceDataList(this PriceDataPointPoco? poco)
    {
        if (poco is null) return null;
        
        var hq = poco.
    }
        
}