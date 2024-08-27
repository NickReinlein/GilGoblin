using System.Collections.Generic;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Fetcher.Pocos;

public record PriceWebPoco(
    int ItemId,
    GeoPriceDataPoco? Hq = null,
    GeoPriceDataPoco? Nq = null,
    List<WorldUploadTimestampPoco>? WorldUploadTimes = null) : IIdentifiable
{
    public int GetId() => ItemId;
}

public record GeoPriceDataPoco(
    PriceDataPerGeoPoco? MinListing,
    PriceDataPerGeoPoco? AverageSalePrice,
    PriceDataPerGeoPoco? RecentPurchase,
    DailySaleVelocityPoco? DailySaleVelocity
);

public record PriceDataPerGeoPoco(
    PriceDataPoco? World = null,
    PriceDataPoco? Dc = null,
    PriceDataPoco? Region = null
);

public record PriceDataPoco(
    float? Price = null,
    int? WorldId = null,
    long? Timestamp = null
);

public record DailySaleVelocityPoco(float? Quantity);

public record WorldUploadTimestampPoco(int? WorldId = null, long? Timestamp = null);