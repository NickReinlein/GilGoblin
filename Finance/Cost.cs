using GilGoblin.Database;
using GilGoblin.Functions;
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
            int base_cost;
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

        public static ItemInfoDB GetItemInfo(int item_id)
        {
            return MarketData.GetItemInfo(item_id);
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
            return GetItemInfo(item_id).iconID;
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
            try
            {
                if (item_id == 0 || world_id == 0){ throw new ParameterException(); }

                ItemDB itemDb = ItemDB.GetItemDataDB(item_id, world_id);
                if (itemDb == null) { return errorReturn; }

                List<RecipeDB> recipesFound = new List<RecipeDB>();
                foreach (RecipeDB recipe in itemDb.recipes)
                {
                    if (recipe == null) { continue; }
                    int recipe_id = recipe.recipe_id;
                    if (recipe_id == 0) { continue; }
                    RecipeFullWeb thisRecipe
                        = RecipeFullWeb.FetchRecipe(recipe_id).GetAwaiter().GetResult();
                    if (thisRecipe == null)
                    {
                        Log.Error("Error fetching recipe id: {recipe_id}.", recipe_id);
                        continue;
                    }
                    recipesFound.Add(thisRecipe.convertToDB());
                }
                if (recipesFound.Count != 0)
                {
                    DatabaseAccess.SaveRecipes(recipesFound).GetAwaiter().GetResult();
                }

                //TODO: check tree traversal here for crafting cost 
                return 7777;
            }
            catch (ParameterException)
            {
                Log.Error("No entry in ItemDB found for: {item_id} world_id: {world_id}. {NewLine}", item_id, world_id);
                return errorReturn;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to fetch the crafting cost for item_id: {item_id} world_id: {world_id}. {NewLine} Error message: {message}", item_id, world_id, ex.Message);
                return errorReturn;
            }
            
        }
    }
}