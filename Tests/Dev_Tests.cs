using GilGoblin.Database;
using GilGoblin.Finance;
using GilGoblin.WebAPI;
using System;

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
        public static async void test_Fetch_Market_Price()
        {
            try
            {
                MarketData marketData = Market.GetMarketData(item_id, world_name);
                ItemInfo itemInfo = Market.GetItemInfo(item_id);

                int marketPrice = marketData.getPrice();
                Console.WriteLine("Final market price: " + marketPrice);
                int vendorPrice = itemInfo.vendor_price;
                Console.WriteLine("Vendor price: " + vendorPrice);
                int profit = marketPrice - vendorPrice;
                Console.WriteLine("Estimated profit: " + profit);

                string success_message = "failure.";
                int db_success = await DatabaseAccess.SaveMarketDataDB(marketData);
                if (db_success > 0) { success_message = "sucess."; }
                Console.WriteLine("Database save was a " + success_message);

                DatabaseAccess.Disconnect();
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
