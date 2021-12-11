using GilGoblin.WebAPI;
using System;

namespace GilGoblin.Finance
{
    internal class Price
    {
        private static Random random_gen = new Random();
        public static MarketData Get_Market_Data(int item_id, string world_name)
        {
            MarketData marketData =
                Market.Get_Market_Data(item_id, world_name).GetAwaiter().GetResult();
            return marketData;
        }

        public static int Get_Market_Price(int item_id, string world_name)
        {
            return Get_Market_Data(item_id, world_name).get_Price();
        }

    }
}