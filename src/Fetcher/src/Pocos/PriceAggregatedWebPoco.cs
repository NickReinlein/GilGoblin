using System.Text.Json.Serialization;
using System.Collections.Generic;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Fetcher.Pocos
{
    public record PriceAggregatedWebPoco(
        int ItemId,
        PriceDataPoco? HQ,
        PriceDataPoco? NQ,
        List<WorldUploadTimestampPoco>? WorldUploadTimes) : IIdentifiable
    {
        public int GetId() => ItemId;
    }

    public record PriceDataPoco(
        MinListingPoco? MinListing,
        RecentPurchasePoco? RecentPurchase,
        AverageSalePriceDataPoco? AverageSalePrice,
        DailySaleVelocityPoco? DailySaleVelocity
    );

    public record MinListingPoco(
        PriceAggregatedPriceDetail? World,
        PriceAggregatedPriceDetail? Dc,
        PriceAggregatedPriceDetail? Region
    );

    public record RecentPurchasePoco(
        PriceAggregatedPriceDetailWithTimestamp? World,
        PriceAggregatedPriceDetailWithTimestamp? Dc,
        PriceAggregatedPriceDetailWithTimestamp? Region
    );

    public record AverageSalePriceDataPoco(float? Price);

    public record DailySaleVelocityPoco(int? SalesPerDay);

    public record PriceAggregatedPriceDetail(float? Price);

    public record PriceAggregatedPriceDetailWithTimestamp(float? Price, long? Timestamp, int? WorldId);

    public record WorldUploadTimestampPoco(int WorldId, long Timestamp);
}