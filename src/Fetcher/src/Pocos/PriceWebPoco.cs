using System.Collections.Generic;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Fetcher.Pocos;

public record PriceWebPoco(
    int ItemId,
    PriceDataPoco? Hq = null,
    PriceDataPoco? Nq = null,
    List<WorldUploadTimestampPoco>? WorldUploadTimes = null) : IIdentifiable
{
    public int GetId() => ItemId;
}

public record PriceDataPoco(
    PriceGeoDataPointsPoco? MinListing,
    RecentPurchasePoco? RecentPurchase,
    PriceGeoDataPointsPoco? AverageSalePrice,
    DailySaleVelocityPoco? DailySaleVelocity
);

public record PriceGeoDataPointsPoco(
    PriceDataPointPoco? World,
    PriceDataPointPoco? Dc,
    PriceDataPointPoco? Region
);

public record RecentPurchasePoco(
    PriceDataPointForGeoPoco? World,
    PriceDataPointForGeoPoco? Dc,
    PriceDataPointForGeoPoco? Region
);

public record DailySaleVelocityPoco(float? Quantity);

public record PriceDataPointPoco(float? Price = null, int? WorldId = null);

public record PriceDataPointForGeoPoco(float? Price = null, long? Timestamp = null, int? WorldId = null)
    : PriceDataPointPoco(Price, WorldId);

public record WorldUploadTimestampPoco(int? WorldId = null, long? Timestamp = null);