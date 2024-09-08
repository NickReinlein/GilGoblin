namespace GilGoblin.Database.Pocos;

public record DailySaleVelocityDbPoco(
    int Id,
    float? WorldQuantity,
    float? DcQuantity,
    float? RegionQuantity,
    int id,
    int itemId,
    bool isHq,
    decimal? worldQuantity,
    decimal? dcQuantity,
    decimal? regionQuantity)
    : DailySaleVelocityWebPoco(Id, WorldQuantity, DcQuantity, RegionQuantity)
{
    public int ItemId { get; set; } = itemId;
    public bool IsHq { get; set; } = isHq;
}