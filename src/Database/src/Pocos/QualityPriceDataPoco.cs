namespace GilGoblin.Database.Pocos;

public record QualityPriceDataPoco(
    PriceDataPointPoco? AverageSalePrice,
    PriceDataPointPoco? MinListing,
    PriceDataPointPoco? RecentPurchase,
    DailySaleVelocityPoco? DailySaleVelocity);