using GilGoblin.Database;
using GilGoblin.Finance;
using GilGoblin.WebAPI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GilGoblin.Tests
{
    public static class testCalcs
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
        public static async void test_Top_Crafts()
        {
            int topNumberOfCrafts = 20;
            System.Console.WriteLine("What is your world id? (ie: 34 for Brynhildr)");
            int intWorldID = Convert.ToInt32(Console.ReadLine());
            if (!(intWorldID >= 0 && intWorldID <= 200))
            {
                System.Console.WriteLine("Not a valid choice!");
                return;
            }
            System.Console.WriteLine("Choose the crafting class:");
            System.Console.WriteLine("1.Carpenter");
            System.Console.WriteLine("2.Blacksmith");
            System.Console.WriteLine("3.Armorer");
            System.Console.WriteLine("4.Goldsmith"); 
            System.Console.WriteLine("5.Leatherworker");
            System.Console.WriteLine("6.Weaver");
            System.Console.WriteLine("7.Alchemist");
            System.Console.WriteLine("8.Culinarian");
            int intCraft = Convert.ToInt32(Console.ReadLine());
            if (!(intCraft >=1 && intCraft <= 8)) {
                System.Console.WriteLine("Not a valid choice!");
                return; 
            }
            else
            {

            }
            CraftingClass craftingClass = new CraftingClass();
            List<int> craftables = CraftingList.getListOfCraftableItemIDsByClass(craftingClass);

            List<ItemDB> craftableItems = ItemDB.GetItemDBBulk(craftables, intWorldID);
            if (craftableItems == null || craftableItems.Count == 0){
                System.Console.WriteLine("No items found!");
                Log.Fatal("Could not find craftable items despite a supposedly correct crafting class integer.");
                return;
            }

            List<MarketDataDB> marketData = MarketDataDB.GetMarketDataBulk(craftables, intWorldID,true);

            Dictionary<int, int> craftsList = new Dictionary<int, int>();
            Dictionary<int, Prices> craftsPrices = new Dictionary<int, Prices>();
            foreach (ItemDB craft in craftableItems){
                Prices craftPrice = new Prices(craft.itemID, intWorldID);
                craftsPrices.Add(craft.itemID, craftPrice);
                craftsList.Add(craft.itemID, craftPrice.profit);
            }

            var topList = craftsList.OrderByDescending(t => t.Value).Take(topNumberOfCrafts).ToList();

            int count = 0;
            foreach (var top in topList){
                count++;
                int itemID = top.Key;
                ItemDB item = craftableItems[itemID];
                Prices prices = craftsPrices[itemID];
                Console.WriteLine("#" + count + ". " + item.itemInfo.name + "  . Avg. price:" + prices.averagePrice + " - crafting cost:" + prices.craftingCost + " = profit:" + prices.profit + " Gil.");
            }
        }
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
                    .MinimumLevel.Debug() // PROD: reduce verbosity
                    .WriteTo.Console()  
                    .WriteTo.File("logs/test.txt")
                    .CreateLogger();

                Log.Debug("Application started.");

                DatabaseAccess.Startup();

                List<int> fetchIDList
                    = new List<int> { 3057, 34680, 5057, 5114, 5106, 3256 };                    

                foreach (int id in fetchIDList) 
                {
                    //Console.WriteLine();
                    //int cost = Cost.GetCraftingCost(id, defaultWorldID);
                    //Log.Debug("Calculated crafting cost for item {itemID} on world {worldID} is: {cost}", id, defaultWorldID, cost);
                    //int averagePrice = Price.getAveragePrice(id, defaultWorldID);
                    //Log.Debug("Calculated average price for item {itemID} on world {worldID} is: {price}", id, defaultWorldID, averagePrice);
                    //int vendorCost = Cost.GetVendorCost(id);
                    //Log.Debug("Vendor cost for item {itemID} is: {vendorCost}.",id,vendorCost);
                    //int baseCost = Cost.GetBaseCost(id, defaultWorldID);
                    //Log.Debug("Calculated base cost for item {itemID} on world {worldID} is: {baseCost}", id, defaultWorldID, baseCost);
                    //int minCost = Cost.GetMinCost(id,defaultWorldID);
                    //Log.Debug("Calculated min cost for item {itemID} on world {worldID} is: {baseCost}", id, defaultWorldID, minCost);
                    //int profit = averagePrice - baseCost;
                    //Log.Debug("Calculated profit for item {itemID} on world {worldID} is: {profit}", id, defaultWorldID, profit);
                    //Console.WriteLine();
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

    internal class IOrderedEnumerable<T1, T2> {
        public static implicit operator IOrderedEnumerable<T1, T2>(SortedList<int, int> v)
        {
            throw new NotImplementedException();
        }
    }
}
