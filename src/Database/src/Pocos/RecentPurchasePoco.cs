namespace GilGoblin.Database.Pocos;

public record RecentPurchasePoco(
    int ItemId,
    bool IsHq,
    int? WorldDataPointId,
    int? DcDataPointId,
    int? RegionDataPointId)
    : PriceDataPointPoco(ItemId,
        IsHq,
        WorldDataPointId,
        DcDataPointId,
        RegionDataPointId);