using System;
using GilGoblin.WebAPI;

namespace GilGoblin.Finance
{
    internal class Cost
    { 
        private static Random random_gen = new Random();
        /// <summary>
        /// Returns the base cost based on the lower of the market price and
        /// the crafted cost of the recipe's base items, using tree traversal
        /// </summary>
        /// <returns></returns>
        public static int Calculate_Base_Cost(int item_id, bool ignore_limited_vendor_qty = false)
        {
            int base_cost = 0;           
            int crafting_cost = Get_Crafting_Cost(item_id);
            int vendor_cost = Get_Vendor_Cost(item_id);

            //TODO: fetch market price from API and/or calculate from a recipe
            // for now we pretend to have one and use a random number       
            // If the item is available at a vendor at a lower cost, use that instead
            if (vendor_cost > 0 && !ignore_limited_vendor_qty)
            {
                base_cost = (int)Math.Min(crafting_cost, vendor_cost);
            }

            return base_cost;
        }
        ///
        public static int Get_Vendor_Cost(int item_id)
        {
            return Market.Get_Item_Info(item_id).GetAwaiter().GetResult();
        }

        public static int Get_Base_Cost(int item_id)
        {
            int base_cost = Calculate_Base_Cost(item_id);

            // for now we pretend to have one and use a random number                       
            if (base_cost == 0)
            {
                base_cost = random_gen.Next(200, 700);
            }

            return base_cost;
        }

        /// <summary>
        /// Get the recipe of crafting components, then using binary tree traversal,
        /// calculate the total cost of crafting the item
        /// </summary>
        /// <returns></returns>
        public static int Get_Crafting_Cost(int item_id)
        {
            return 0;
        }
    }
}