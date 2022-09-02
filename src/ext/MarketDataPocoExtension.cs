using GilGoblin.pocos;

namespace GilGoblin.ext
{
    public static class MarketDataPocoExtension
    {
        //public static int GetCraftingCost(){};
        public static int GetAverageSoldPrice(this MarketDataPoco marketData, bool hq = false)
        {
            return hq ? (int)marketData.AverageSoldHQ : (int)marketData.AverageSoldNQ;
        }

        public static int GetAverageListingPrice(this MarketDataPoco marketData, bool hq = false)
        {
            return hq
                ? (int)marketData.AverageListingPriceHQ
                : (int)marketData.AverageListingPriceNQ;
        }
    }
}
