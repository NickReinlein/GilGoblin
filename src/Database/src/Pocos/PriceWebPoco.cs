using System.Collections.Generic;

namespace GilGoblin.Database.Pocos;

public record PriceWebPoco(
    int ItemId,
    QualityPriceDataWebPoco? Hq = null,
    QualityPriceDataWebPoco? Nq = null,
    List<WorldUploadTimeWebPoco>? WorldUploadTimes = null)
    : IIdentifiable
{
    public int GetId() => ItemId;
}

public record QualityPriceDataWebPoco(
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
    decimal Price,
    int? WorldId = null,
    long? Timestamp = null
);

public record DailySaleVelocityWebPoco(
    SaleQuantity? World,
    SaleQuantity? Dc,
    SaleQuantity? Region
);

public record SaleQuantity(decimal Quantity);

public record WorldUploadTimeWebPoco(int WorldId, long Timestamp);