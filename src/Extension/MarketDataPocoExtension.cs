using GilGoblin.Pocos;

namespace GilGoblin.Ext;

public static class PricePocoExtension
{
    public static int GetAverageSoldPrice(this PricePoco price, bool hq = false)
    {
        return hq ? (int)price.AverageSoldHQ : (int)price.AverageSoldNQ;
    }

    public static int GetAverageListingPrice(this PricePoco price, bool hq = false)
    {
        return hq ? (int)price.AverageListingPriceHQ : (int)price.AverageListingPriceNQ;
    }
}
