namespace GilGoblin.Database.Pocos;

public record DailySaleVelocityPoco(
    int Id,
    int ItemId,
    bool IsHq,
    WebPocoQuantity? World,
    WebPocoQuantity? Dc,
    WebPocoQuantity? Region)
    : DailySaleVelocityWebPoco(World, Dc, Region)
{
    public DailySaleVelocityPoco(int id, int itemId, bool isHq, float worldQuantity, float dcQuantity, float regionQuantity)
        : this(id, itemId, isHq,
            new WebPocoQuantity(worldQuantity),
            new WebPocoQuantity(dcQuantity),
            new WebPocoQuantity(regionQuantity))
    {
    }
}