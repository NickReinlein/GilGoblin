using GilGoblin.Database;
using System;

namespace GilGoblin.Finance
{
    public class Prices {
        private static Random random_gen = new Random();

        public int itemID { get; private set; }
        public int worldID { get; private set; }
        public int averagePrice { get; private set; } = 0 ;
        public int craftingCost { get; private set; } = 0;
        public int profit { get; private set; } = 0;

        public Prices() { }
        public Prices(int itemID, int worldID)
        {
            this.itemID = itemID;
            this.averagePrice = getAveragePrice(itemID, worldID);
            this.craftingCost = Cost.GetCraftingCost(itemID, worldID);
            this.profit = this.averagePrice-this.craftingCost;
        }

        public Prices(int itemID, int worldID, int averagePrice, int craftingCost, int profit)
        {
            this.itemID = itemID;   
            this.worldID = worldID;
            this.averagePrice = averagePrice;
            this.craftingCost = craftingCost;
            this.profit = profit;
        }

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