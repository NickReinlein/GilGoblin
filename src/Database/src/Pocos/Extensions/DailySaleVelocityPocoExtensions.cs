namespace GilGoblin.Database.Pocos.Extensions;

public static class DailySaleVelocityPocoExtensions
{
    public static bool HasAValidQuantity(this DailySaleVelocityWebPoco? poco) =>
        poco?.WorldQuantity >= 0 ||
        poco?.DcQuantity >= 0 ||
        poco?.RegionQuantity >= 0;
}