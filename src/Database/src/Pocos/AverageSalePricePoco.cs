namespace GilGoblin.Database.Pocos;

public record AverageSalePricePoco(
    int Id,
    int ItemId,
    bool IsHq,
    int? WorldDataPointId,
    int? DcDataPointId,
    int? RegionDataPointId)
    : PriceDataPointPoco(Id, ItemId, IsHq, WorldDataPointId,
        DcDataPointId, RegionDataPointId);