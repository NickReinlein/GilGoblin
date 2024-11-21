using System;

namespace GilGoblin.Database.Pocos.Extensions;

public static class PriceDataPointWebPocoExtensions
{
    public static bool HasValidPrice(this PriceDataPointWebPoco? poco) =>
        poco is not null &&
        (poco.Dc.HasValidPrice() ||
         poco.Region.HasValidPrice() ||
         poco.World.HasValidPrice());

    public static T? AsPoco<T>(this PriceDataPointPoco? poco, Func<PriceDataPointPoco, T> factory)
        where T : class
        => poco is null ? null : factory(poco);

    public static RecentPurchasePoco? AsRecentPurchasePoco(this PriceDataPointPoco? poco) =>
        poco.AsPoco(recent => new RecentPurchasePoco(
            recent.ItemId,
            recent.WorldId,
            recent.IsHq,
            recent.WorldDataPointId,
            recent.DcDataPointId,
            recent.RegionDataPointId));

    public static AverageSalePricePoco? AsAverageSalePricePoco(this PriceDataPointPoco? poco) =>
        poco.AsPoco(average => new AverageSalePricePoco(
            average.ItemId,
            average.WorldId,
            average.IsHq,
            average.WorldDataPointId,
            average.DcDataPointId,
            average.RegionDataPointId));

    public static MinListingPoco? AsMinListingPoco(this PriceDataPointPoco? poco) =>
        poco.AsPoco(min => new MinListingPoco(
            min.ItemId,
            min.WorldId,
            min.IsHq,
            min.WorldDataPointId,
            min.DcDataPointId,
            min.RegionDataPointId));
}