using GilGoblin.pocos;

namespace GilGoblin.ext
{
    public static class MarketDataPocoExtension
    {
        //public static int GetCraftingCost(){};
        public static int GetAverageSoldPrice(this MarketDataPoco marketData, bool hq = false)
        {
            return hq ? (int)marketData.averageSaleHQ : (int)marketData.averageSaleHQ;
        }

        public static int GetAverageListingPrice(this MarketDataPoco marketData, bool hq = false)
        {
            return hq ? (int)marketData.averagePriceHQ : (int)marketData.averagePriceNQ;
        }
    }
}
