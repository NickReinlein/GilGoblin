using System.Collections.Generic;

namespace GilGoblin.Database.Pocos;

public record PriceWebPoco(
    int ItemId,
    QualityPriceDataPoco? Hq = null,
    QualityPriceDataPoco? Nq = null,
    List<WorldUploadTimeWebPoco>? WorldUploadTimes = null)
    : IIdentifiable
{
    public int GetId() => ItemId;
}

public record QualityPriceDataPoco(
    PriceDataPointWebPoco? MinListing,
    PriceDataPointWebPoco? AverageSalePrice,
    PriceDataPointWebPoco? RecentPurchase,
    DailySaleVelocityWebPoco? DailySaleVelocity
);

public record PriceDataPointWebPoco(
    PriceDataDetailPoco? World,
    PriceDataDetailPoco? Dc,
    PriceDataDetailPoco? Region
);

public record PriceDataDetailPoco(
    float Price,
    int? WorldId = null,
    long? Timestamp = null
);

public record DailySaleVelocityWebPoco(
    int Id,
    float? WorldQuantity,
    float? DcQuantity,
    float? RegionQuantity
);

public record WorldUploadTimeWebPoco(int WorldId, long Timestamp);