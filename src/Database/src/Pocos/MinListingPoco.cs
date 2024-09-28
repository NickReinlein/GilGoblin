namespace GilGoblin.Database.Pocos;

public record MinListingPoco(
    int ItemId,
    bool IsHq,
    int? WorldDataPointId = null,
    int? DcDataPointId = null,
    int? RegionDataPointId = null)
    : PriceDataPointPoco(ItemId,
        IsHq,
        WorldDataPointId,
        DcDataPointId,
        RegionDataPointId);