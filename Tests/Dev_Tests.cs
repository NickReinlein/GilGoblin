using System;
using GilGoblin.Finance;
using GilGoblin.Database;
using GilGoblin.WebAPI;

namespace GilGoblin.Tests
{
    internal static class testCalcs
    {
        // Wolrd ID & Name for testing: 34 	Brynhildr
        // Item ID for Mithril Ore: 5114
        // Item ID for Copper Ore: 5106
        // Item ID for Copper Ingot: 5057

        public const string world_name = "Brynhildr";
        public const int item_id = 5057;
        public static void test_Fetch_Market_Price()
        {
            try
            {
                MarketData marketData = Market.GetMarketData(item_id, world_name);
                ItemInfo itemInfo = Market.GetItemInfo(item_id);

                int marketPrice = marketData.average_Price;
                Console.WriteLine("Final market price: " + marketPrice);
                int vendorPrice = itemInfo.vendor_price;
                Console.WriteLine("Vendor price: " + vendorPrice);
                int profit = marketPrice - vendorPrice;
                Console.WriteLine("Estimated profit: " + profit);

                DatabaseAccess.SaveMarketDataDB(marketData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally 
            { 
                Console.ReadLine();
            }
        }
    }
}
