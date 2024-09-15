namespace GilGoblin.Database.Pocos;

public record AverageSalePricePoco(
    int Id,
    int ItemId,
    bool IsHq,
    int? WorldDataPointId = null,
    int? DcDataPointId = null,
    int? RegionDataPointId = null)
    : PriceDataPointPoco(Id, ItemId, IsHq, WorldDataPointId,
        DcDataPointId, RegionDataPointId);