namespace GilGoblin.Database.Pocos.Extensions;

public static class QualityPriceDataPocoExtensions
{
    public static bool HasValidPrice(this QualityPriceDataWebPoco? poco) =>
        poco is not null &&
        (poco.AverageSalePrice.HasValidPrice() ||
         poco.RecentPurchase.HasValidPrice() ||
         poco.MinListing.HasValidPrice());
}