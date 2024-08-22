using System.Text.Json.Serialization;
using System.Collections.Generic;
using GilGoblin.Database.Pocos;

namespace GilGoblin.Fetcher.Pocos
{
    public record PriceAggregatedWebPoco : BasePricePoco
    {
        public PriceDataPoco? NQ { get; set; }
        public PriceDataPoco? HQ { get; set; }
        public List<WorldUploadTimestampPoco>? WorldUploadTimes { get; set; }

        public PriceAggregatedWebPoco() { }

        [JsonConstructor]
        public PriceAggregatedWebPoco(
            int itemId,
            PriceDataPoco? nq,
            PriceDataPoco? hq,
            List<WorldUploadTimestampPoco>? worldUploadTimes
        )
        {
            ItemId = itemId;
            NQ = nq;
            HQ = hq;
            WorldUploadTimes = worldUploadTimes;
        }
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

    public record PriceAggregatedPriceDetail(float Price);

    public record PriceAggregatedPriceDetailWithTimestamp(float Price, long Timestamp, int? WorldId);

    public record WorldUploadTimestampPoco(int WorldId, long Timestamp);
}