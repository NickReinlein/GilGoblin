namespace GilGoblin.Database.Pocos;

public record PriceDataPointPoco(
    int Id,
    int ItemId,
    bool IsHq,
    int? WorldDataPointId,
    int? DcDataPointId,
    int? RegionDataPointId)
{
    public int Id { get; set; } = Id;
    public int ItemId { get; set; } = ItemId;
    public bool IsHq { get; set; } = IsHq;
    public int? WorldDataPointId { get; set; } = WorldDataPointId;
    public int? DcDataPointId { get; set; } = DcDataPointId;
    public int? RegionDataPointId { get; set; } = RegionDataPointId;
}