namespace GilGoblin.Database.Pocos;

public record QualityPriceDataPoco(
    PriceDataPointPoco? MinListing,
    PriceDataPointPoco? AverageSalePrice,
    PriceDataPointPoco? RecentPurchase,
    DailySaleVelocityPoco? DailySaleVelocity);