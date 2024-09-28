namespace GilGoblin.Database.Pocos.Extensions;

public static class PriceDataPointPocoExtensions
{
    public static MinListingPoco ToMinListing(this PriceDataPointPoco poco)
        => new(poco.ItemId, poco.IsHq, poco.WorldDataPointId, poco.DcDataPointId, poco.RegionDataPointId);

    public static AverageSalePricePoco ToAverageSalePrice(this PriceDataPointPoco poco)
        => new(poco.ItemId, poco.IsHq, poco.WorldDataPointId, poco.DcDataPointId, poco.RegionDataPointId);

    public static RecentPurchasePoco ToRecentPurchase(this PriceDataPointPoco poco)
        => new(poco.ItemId, poco.IsHq, poco.WorldDataPointId, poco.DcDataPointId, poco.RegionDataPointId);
}