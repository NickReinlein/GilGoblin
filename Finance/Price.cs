using GilGoblin.Database;
using System;

namespace GilGoblin.Finance
{
    internal class Price
    {
        private static Random random_gen = new Random();

        public static int getAveragePrice(int itemID, int worldID)
        {
            MarketDataDB marketDataDB = MarketDataDB.GetMarketDataSingle(itemID, worldID);
            if (marketDataDB == null) { return 0; }
            else
            {
                return marketDataDB.averagePrice;
            }
        }
    }
}