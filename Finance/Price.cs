using GilGoblin.WebAPI;
using System;

namespace GilGoblin.Finance
{
    internal class Price
    {
        private static Random random_gen = new Random();
        public static MarketData GetMarketData(int item_id, string world_name)
        {
            MarketData marketData =
                Market.GetMarketData(item_id, world_name).GetAwaiter().GetResult();
            return marketData;
        }

        public static int GetMarketPrice(int item_id, string world_name)
        {
            return GetMarketData(item_id, world_name).get_Price();
        }

    }
}