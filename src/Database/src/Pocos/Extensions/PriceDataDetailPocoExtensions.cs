namespace GilGoblin.Database.Pocos.Extensions;

public static class PriceDataDetailPocoExtensions
{
    public static bool HasValidPrice(this PriceDataDetailPoco? poco) => poco?.Price > 0;
}