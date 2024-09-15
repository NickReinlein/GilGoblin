namespace GilGoblin.Database.Pocos.Extensions;

public static class PriceWebPocoExtensions
{
    public static int GetWorldId(this PriceWebPoco webPoco) =>
        webPoco.Hq?.AverageSalePrice?.Dc?.WorldId
        ?? webPoco.Nq?.AverageSalePrice?.Dc?.WorldId
        ?? webPoco.Hq?.MinListing?.Dc?.WorldId
        ?? webPoco.Nq?.MinListing?.Dc?.WorldId
        ?? webPoco.Hq?.RecentPurchase?.Dc?.WorldId
        ?? webPoco.Nq?.RecentPurchase?.Dc?.WorldId
        ?? 0;
}