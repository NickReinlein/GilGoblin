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
        public const int defaultWorldID = 34;
        public const int defaultItemID = 3057;
        public static async void test_Fetch_Market_Price()
        {
            try
            {
                MarketDataDB marketData = MarketDataDB.GetMarketDataSingle(defaultItemID, defaultWorldID);
                ItemInfoDB itemInfo =  ItemInfoDB.GetItemInfo(defaultItemID);

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
                int craftingCost = Cost.GetCraftingCost(defaultItemID, defaultWorldID);

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
                    .MinimumLevel.Verbose() // PROD: reduce verbosity
                    .WriteTo.Console()  
                    .WriteTo.File("logs/test.txt")
                    .CreateLogger();

                Log.Debug("Application started.");

                DatabaseAccess.Startup();

                List<int> fetchIDList
                    = new List<int> { 3057, 34680, 5057, 5114, 5106, 3256 };                    

                foreach (int id in fetchIDList) 
                {
                    Console.WriteLine();
                    int cost = Cost.GetCraftingCost(id, defaultWorldID);
                    Log.Debug("Calculated crafting cost for item {itemID} on world {worldID} is: {cost}", id, defaultWorldID, cost);
                    int averagePrice = Price.getAveragePrice(id, defaultWorldID);
                    Log.Debug("Calculated average price for item {itemID} on world {worldID} is: {price}", id, defaultWorldID, averagePrice);
                    int vendorCost = Cost.GetVendorCost(id);
                    Log.Debug("Vendor cost for item {itemID} is: {vendorCost}.",id,vendorCost);
                    int baseCost = Cost.GetBaseCost(id, defaultWorldID);
                    Log.Debug("Calculated base cost for item {itemID} on world {worldID} is: {baseCost}", id, defaultWorldID, baseCost);
                    int minCost = Cost.GetMinCost(id,defaultWorldID);
                    Log.Debug("Calculated min cost for item {itemID} on world {worldID} is: {baseCost}", id, defaultWorldID, minCost);
                    int profit = averagePrice - baseCost;
                    Log.Debug("Calculated profit for item {itemID} on world {worldID} is: {profit}", id, defaultWorldID, profit);
                    Console.WriteLine();
                }

                //DatabaseAccess.Save();
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
