using GilGoblin.Database;
using GilGoblin.WebAPI;
using Serilog;
using System;
using System.Collections.Generic;

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
        public static int CalculateBaseCost(int item_id, int world_id, 
            bool ignore_limited_vendor_qty = false)
        {
            int base_cost = 0;
            int crafting_cost = GetCraftingCost(item_id, world_id);
            int vendor_cost = GetVendorCost(item_id);

            // If the item is available at a vendor at a lower cost, use that instead
            if (vendor_cost > 0 && !ignore_limited_vendor_qty)
            {
                base_cost = (int)Math.Min(crafting_cost, vendor_cost);
            }
            else
            {
                base_cost = crafting_cost;
            }

            Log.Debug("For {item_id} in world {world_id}, base cost: {base_cost} from crafting: {crafting_cost} and vendor: {vendor_cost}");

            return base_cost;
        }
        ///

        public static ItemInfo GetItemInfo(int item_id)
        {
            return MarketDataWeb.GetItemInfo(item_id);
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

        public static int GetBaseCost(int item_id, int world_id)
        {
            int base_cost = CalculateBaseCost(item_id, world_id);

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
        public static int GetCraftingCost(int item_id, int world_id)
        {
            int errorReturn = 999999;
            MarketDataDB db = MarketData.GetMarketData(item_id,world_id);
            ItemInfo itemInfo = MarketData.GetItemInfo(item_id);
            if (db == null || itemInfo == null)
            {
                Log.Error("Failed to fetch the crafting cost for item_id: {item_id} world_id: {world_id}");
                return errorReturn;
            }

            List<RecipeFullWeb> recipesFound = new List<RecipeFullWeb>();
            foreach (ItemRecipeShortAPI recipe in itemInfo.recipes)
            {
                int recipe_id = recipe.recipe_id;
                if (recipe_id == 0) { continue; }
                RecipeFullWeb thisRecipe 
                    = RecipeFullWeb.FetchRecipe(recipe_id).GetAwaiter().GetResult();
                if (thisRecipe == null)
                {
                    Log.Error("Error fetching recipe id: {recipe_id}.", recipe_id);
                    continue;
                }
                recipesFound.Add(thisRecipe);                 
            }
            if (recipesFound.Count != 0)
            {
                DatabaseAccess.SaveRecipes(recipesFound);
            }

            //TODO: check tree traversal here for crafting cost 
            return 0;
            
        }
    }
}