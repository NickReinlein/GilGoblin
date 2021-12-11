using System;
using GilGoblin.Finance;
using GilGoblin.Database;

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
            int marketPrice = Price.GetMarketPrice(item_id, world_name);
            Console.WriteLine("Final market price: " + marketPrice);
            int vendorPrice = Cost.GetVendorCost(item_id);
            Console.WriteLine("Vendor price: " + vendorPrice);
            int profit = marketPrice - vendorPrice;
            Console.WriteLine("Estimated profit: " + profit);

            try
            {
                DatabaseAccess.Connect();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }

            Console.ReadLine();
        }
    }
}
