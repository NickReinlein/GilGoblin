namespace GilGoblin.Database.Pocos.Extensions;

public static class PriceDataPocoExtensions
{
    public static bool HasValidPrice(this PriceDataPoco? poco) =>
        poco is not null &&
        (poco.Dc.HasValidPrice() ||
         poco.Region.HasValidPrice() ||
         poco.World.HasValidPrice());
}