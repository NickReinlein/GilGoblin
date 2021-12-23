using GilGoblin.Database;
using GilGoblin.Finance;
using GilGoblin.WebAPI;
using Serilog;
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
                ItemInfoDB itemInfo =  MarketData.GetItemInfo(item_id);

                if (marketData == null || itemInfo == null)
                {
                    throw new Exception("Market data or item info not found");
                }
                int marketPrice = marketData.average_price;
                Log.Information("Final market price: " + marketPrice);
                int vendorPrice = itemInfo.vendor_price;
                Log.Information("Vendor price: " + vendorPrice);
                int profit = marketPrice - vendorPrice;
                Log.Information("Estimated profit: " + profit);

                string success_message = "failure.";
                int db_success = await DatabaseAccess.SaveMarketDataSingle(marketData);
                if (db_success > 0) { success_message = "sucess."; }
                Log.Information("Database save was a " + success_message);

                //Test crafting cost calculation by tree traversal
                int craftingCost = Cost.GetCraftingCost(item_id, world_id);

                DatabaseAccess.Disconnect();
            }
            catch (Exception ex)
            {
                Log.Error("Exception: {message}.", ex.Message);
            }
            finally
            {
                Log.Information("Press any button to quit.");
                Console.ReadLine();
            }
        }

        //Bulk test
        public static async void test_Fetch_Market_Prices()
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug()
                    .WriteTo.Console()  
                    .WriteTo.File("logs/test.txt")
                    .CreateLogger();

                Log.Debug("Application started.");

                List<int> fetchIDList = new List<int> { item_id, 5114, 5106 };
                List<ItemInfo> infoList = new List<ItemInfo>();
                List<MarketDataDB> marketDataList 
                    = MarketData.GetMarketDataBulk(fetchIDList, world_id, true);
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
                Log.Information("Database save was a " + success_message);
                Log.Information(db_success + " entries saved to the database.");

                foreach (int id in fetchIDList) 
                {
                    Cost.GetCraftingCost(id, world_id);
                }

                DatabaseAccess.Disconnect();
                Log.Information("Application ended successfully.");
                Log.CloseAndFlush();
            }
            catch (Exception ex)
            {
                Log.Error("Exception: {message}.", ex.Message);
                Log.CloseAndFlush();
            }
            finally
            {
                Console.WriteLine("Press any button to quit.");
                Console.ReadLine();
            }
        }
    }
}
