namespace GilGoblin.Database.Pocos;

public record DailySaleVelocityPoco(
    int Id,
    int ItemId,
    bool IsHq,
    float? WorldQuantity,
    float? DcQuantity,
    float? RegionQuantity)
    : DailySaleVelocityWebPoco(Id, WorldQuantity, DcQuantity, RegionQuantity);