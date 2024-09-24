namespace GilGoblin.Database.Pocos;

public record DailySaleVelocityPoco(
    int Id,
    int ItemId,
    bool IsHq,
    WebPocoQuantity? WorldQuantity,
    WebPocoQuantity? DcQuantity,
    WebPocoQuantity? RegionQuantity)
    : DailySaleVelocityWebPoco(WorldQuantity, DcQuantity, RegionQuantity)
{
    public DailySaleVelocityPoco(int id, int itemId, bool isHq, float worldQuantity, float dcQuantity, float regionQuantity)
        : this(id, itemId, isHq,
            new WebPocoQuantity(worldQuantity),
            new WebPocoQuantity(dcQuantity),
            new WebPocoQuantity(regionQuantity))
    {
    }
}