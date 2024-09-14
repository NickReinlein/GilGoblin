namespace GilGoblin.Database.Pocos.Extensions;

public static class PriceWebPocoExtensions
{
    public static int GetWorldId(this PriceWebPoco webPoco) =>
        webPoco.Hq?.AverageSalePrice?.World?.WorldId
        ?? webPoco.Nq?.AverageSalePrice?.World?.WorldId
        ?? webPoco.Hq?.MinListing?.World?.WorldId
        ?? webPoco.Nq?.MinListing?.World?.WorldId
        ?? 0;
}