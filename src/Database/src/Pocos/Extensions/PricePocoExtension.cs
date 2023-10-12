namespace GilGoblin.Database.Pocos.Extensions;

public static class PricePocoExtensions
{
    public static int GetAverageSoldPrice(this PricePoco price, bool hq = false) =>
        hq ? (int)price.AverageSoldHQ : (int)price.AverageSoldNQ;

    public static int GetAverageListingPrice(this PricePoco price, bool hq = false) =>
        hq ? (int)price.AverageListingPriceHQ : (int)price.AverageListingPriceNQ;
}
