namespace GilGoblin.Database.Pocos;

public record RecentPurchasePoco(
    int ItemId,
    int WorldId,
    bool IsHq,
    int? WorldDataPointId = null,
    int? DcDataPointId = null,
    int? RegionDataPointId = null)
    : PriceDataPointPoco(
        ItemId,
        WorldId,
        IsHq,
        WorldDataPointId,
        DcDataPointId,
        RegionDataPointId);