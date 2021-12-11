using GilGoblin.WebAPI;
using System;

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
        public static int CalculateBaseCost(int item_id, bool ignore_limited_vendor_qty = false)
        {
            int base_cost = 0;
            int crafting_cost = GetCraftingCost(item_id);
            int vendor_cost = GetVendorCost(item_id);

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

        public static ItemInfo GetItemInfo(int item_id)
        {
            return Market.FetchItemInfo(item_id).GetAwaiter().GetResult();
        }
        public static int GetVendorCost(int item_id)
        {
            return GetItemInfo(item_id).vendor_price;

        }
        public static string GetDescription(int item_id)
        {
            return GetItemInfo(item_id).description;

        }
        public static int GetIconID(int item_id)
        {
            return GetItemInfo(item_id).icon_id;

        }

        public static int GetBaseCost(int item_id)
        {
            int base_cost = CalculateBaseCost(item_id);

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
        public static int GetCraftingCost(int item_id)
        {
            return 0;
        }
    }
}