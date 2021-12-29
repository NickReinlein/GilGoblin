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
        // Item ID for Chondrite Magitek Sword: 34680
        // Item ID for Bomb Frypan: 2499
        // Item ID for Iron Scale Mail: 3057
        // Item ID for Heavy Darksteel Armor: 3256

        public const string world_name = "Brynhildr";
        public const int world_id = 34;
        public const int item_id = 3057;
        public static async void test_Fetch_Market_Price()
        {
            try
            {
                MarketDataDB marketData = MarketDataDB.GetMarketDataSingle(item_id, world_id);
                ItemInfoDB itemInfo =  ItemInfoDB.GetItemInfo(item_id);

                if (marketData == null || itemInfo == null)
                {
                    throw new Exception("Market data or item info not found");
                }
                float marketPrice = marketData.averagePrice;
                Log.Information("Final market price: " + marketPrice);
                int vendorPrice = itemInfo.vendor_price;
                Log.Information("Vendor price: " + vendorPrice);
                int profit = (int)marketPrice - vendorPrice;
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
        public static void test_Fetch_Market_Prices()
        {
            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Debug() // PROD: reduce verbosity
                    .WriteTo.Console()  
                    .WriteTo.File("logs/test.txt")
                    .CreateLogger();

                Log.Debug("Application started.");

                List<int> fetchIDList
                    = new List<int> { 2499, 34680, 5057, 5114, 5106, 3256 };                    

                foreach (int id in fetchIDList) 
                {
                    int cost = Cost.GetCraftingCost(id, world_id);
                    Log.Debug("Calculated crafting cost for item {itemID} on world {worldID} is: {cost}", id, world_id, cost);
                    int averagePrice = Price.getAveragePrice(id, world_id);
                    Log.Debug("Calculated average price for item {itemID} on world {worldID} is: {price}", id, world_id, averagePrice);
                    int baseCost = Cost.GetBaseCost(id, world_id);
                    Log.Debug("Calculated base cost for item {itemID} on world {worldID} is: {baseCost}", id, world_id, baseCost);
                    int profit = averagePrice - baseCost;
                    Log.Debug("Calculated profit for item {itemID} on world {worldID} is: {profit}", id, world_id, profit);

                }

                DatabaseAccess.Save();
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
