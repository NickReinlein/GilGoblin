namespace GilGoblin.Database.Pocos.Extensions;

public static class PriceDataPointWebPocoExtensions
{
    public static bool HasValidPrice(this PriceDataPointWebPoco? poco) =>
        poco is not null &&
        (poco.Dc.HasValidPrice() ||
         poco.Region.HasValidPrice() ||
         poco.World.HasValidPrice());
}