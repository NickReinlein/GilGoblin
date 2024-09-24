namespace GilGoblin.Database.Pocos.Extensions;

public static class DailySaleVelocityPocoExtensions
{
    public static bool HasAValidQuantity(this DailySaleVelocityWebPoco? poco) =>
        poco?.WorldQuantity?.Quantity >= 0 ||
        poco?.DcQuantity?.Quantity >= 0 ||
        poco?.RegionQuantity?.Quantity >= 0;
}