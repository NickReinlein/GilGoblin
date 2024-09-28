namespace GilGoblin.Database.Pocos;

public record PriceDataPointPoco(
    int ItemId,
    bool IsHq,
    int? WorldDataPointId = null,
    int? DcDataPointId = null,
    int? RegionDataPointId = null)
    : IdentifiablePoco;