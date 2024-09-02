using System.Collections.Generic;

namespace GilGoblin.Database.Pocos;

public record BasePricePoco(
    int ItemId,
    QualityPriceDataPoco? Hq = null,
    QualityPriceDataPoco? Nq = null,
    List<WorldUploadTimestampPoco>? WorldUploadTimes = null)
    : IIdentifiable
{
    public int GetId() => ItemId;
}

public record QualityPriceDataPoco(
    PriceDataPoco? MinListing,
    PriceDataPoco? AverageSalePrice,
    PriceDataPoco? RecentPurchase,
    DailySaleVelocityPoco? DailySaleVelocity
);

public record PriceDataPoco(
    PriceDataDetailPoco? World,
    PriceDataDetailPoco? Dc,
    PriceDataDetailPoco? Region
);

public record PriceDataDetailPoco(
    float Price,
    int? WorldId = null,
    long? Timestamp = null
);

public record DailySaleVelocityPoco(
    int Id,
    float? WorldQuantity,
    float? DcQuantity,
    float? RegionQuantity
);

public record WorldUploadTimestampPoco(int WorldId, long Timestamp);