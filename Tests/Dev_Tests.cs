using GilGoblin.Database;
using GilGoblin.Finance;
using GilGoblin.WebAPI;
using System;
using System.Collections.Generic;

namespace GilGoblin.Tests
{
    internal static class testCalcs
    {
        // Wolrd ID & Name for testing: 34 	Brynhildr
        // Item ID for Mithril Ore: 5114
        // Item ID for Copper Ore: 5106
        // Item ID for Copper Ingot: 5057

        public const string world_name = "Brynhildr";
        public const int world_id = 34;
        public const int item_id = 5057;
        public static async void test_Fetch_Market_Price()
        {
            try
            {
                MarketDataDB marketData = MarketData.GetMarketData(item_id, world_id);
                ItemInfo itemInfo = MarketData.GetItemInfo(item_id);

                if (marketData == null || itemInfo == null)
                {
                    throw new Exception("Market data or item info not found");
                }
                int marketPrice = marketData.average_Price;
                Console.WriteLine("Final market price: " + marketPrice);
                int vendorPrice = itemInfo.vendor_price;
                Console.WriteLine("Vendor price: " + vendorPrice);
                int profit = marketPrice - vendorPrice;
                Console.WriteLine("Estimated profit: " + profit);

                string success_message = "failure.";
                int db_success = await DatabaseAccess.SaveMarketData(marketData);
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
                Console.WriteLine("Press any button to quit.");
                Console.ReadLine();
            }
        }

        //Bulk test
        public static async void test_Fetch_Market_Prices()
        {
            try
            {
                Dictionary<int, int> fetchList = new Dictionary<int, int>();
                List<ItemInfo> infoList = new List<ItemInfo>();
                fetchList.Add(item_id, world_id);
                fetchList.Add(5114, world_id);
                fetchList.Add(5106, world_id);
                List<MarketDataDB> marketDataList = MarketData.GetMarketDataBulk(fetchList);
                foreach (MarketDataDB dataDb in marketDataList)
                {
                    infoList.Add(MarketData.GetItemInfo(item_id));
                }

                if (infoList.Count == 0 || marketDataList.Count == 0)
                {
                    throw new Exception("Market data or item info not found");
                }

                string success_message = "failure.";
                int db_success = await DatabaseAccess.SaveMarketDataBulk(marketDataList);
                if (db_success > 0) { success_message = "sucess."; }
                Console.WriteLine("Database save was a " + success_message);
                Console.WriteLine(db_success + " entries saved to the database.");

                DatabaseAccess.Disconnect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                Console.WriteLine("Press any button to quit.");
                Console.ReadLine();
            }
        }
    }
}
