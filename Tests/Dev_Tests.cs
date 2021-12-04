using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GilGoblin.Finance;
using GilGoblin.WebAPI;

namespace GilGoblin.Tests
{
    internal static class test_Calcs
    {
        // Wolrd ID & Name for testing: 34 	Brynhildr
        // Item ID for Mithril Ore: 5114
        public const string world_name = "Brynhildr";
        public const int item_id = 5114;
        public static void test_Fetch_Market_Price()
        {
            Market.Fetch_Market_Price(item_id, world_name);
        }
    }
}
