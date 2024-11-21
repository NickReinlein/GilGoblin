namespace GilGoblin.Database.Pocos.Extensions;

public static class DailySaleVelocityPocoExtensions
{
    public static bool HasAValidQuantity(this DailySaleVelocityWebPoco? poco) =>
        poco?.World?.Quantity >= 0 ||
        poco?.Dc?.Quantity >= 0 ||
        poco?.Region?.Quantity >= 0;
}