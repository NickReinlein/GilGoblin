using GilGoblin.Functions;
using GilGoblin.Database;
using GilGoblin.WebAPI;
using Serilog;
using System;
using System.Collections.Generic;

namespace GilGoblin.Finance
{
    internal class Cost
    {
        public static readonly int _default_world_id = 34;
        private static Random random_gen = new Random();

        /// <summary>
        /// Returns the base cost based on the lower of the market price and
        /// the crafted cost of the recipe's base items, using tree traversal
        /// </summary>
        /// <returns></returns>
        public static int CalculateBaseCost(int itemID, int worldID,
            bool ignore_limited_vendor_qty = false)
        {
            int baseCost;
            int craftingCost = GetCraftingCost(itemID, worldID);
            int vendorCost = GetVendorCost(itemID);

            // If the item is available at a vendor at a lower cost, use that instead
            if (vendorCost > 0 && !ignore_limited_vendor_qty)
            {
                baseCost = (int)Math.Min(craftingCost, vendorCost);
            }
            else
            {
                baseCost = craftingCost;
            }

            return baseCost;
        }
        ///

        public static ItemInfoDB GetItemInfo(int item_id)
        {
            return ItemInfoDB.GetItemInfo(item_id);
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
            return base_cost;
        }

        public static int GetMinCost(int itemID, int worldID)
        {
            int baseCost = CalculateBaseCost(itemID, worldID);
            int craftingCost = GetCraftingCost(itemID, worldID);
            return Math.Min(baseCost, craftingCost);
        }

        /// <summary>
        /// Get the recipe of crafting components, then using binary tree traversal,
        /// calculate the total cost of crafting the item
        /// </summary>
        /// <returns></returns>
        public static int GetCraftingCost(int itemID, int worldID)
        {
            int errorReturn = 999999;
            int craftingCost = 0;
            try
            {
                if (itemID == 0 || worldID == 0) { throw new ParameterException(); }

                ItemDB itemDB;
                List<RecipeDB> recipesFetched = new List<RecipeDB>();

                try
                {
                    itemDB = ItemDB.GetItemDBSingle(itemID, worldID);
                }
                catch (Exception)
                {
                    Log.Debug("No entry in ItemDB found for: {item_id} world_id: {world_id}. {NewLine}", itemID, worldID);
                    itemDB = null;
                }

                if (itemDB == null)
                {
                    itemDB = ItemDB.FetchItemDBSingle(itemID, worldID);
                    if (itemDB == null)
                    {
                        throw new Exception();
                    }
                }

                if (itemDB != null){
                    if (itemDB.fullRecipes.Count > 0)
                    {
                        //using (ItemDBContext context = DatabaseAccess.getContext()) { 
                        //    DatabaseAccess.SaveRecipes(itemDB.fullRecipes).GetAwaiter().GetResult();
                        //}

                        RecipeIngredientBreakdown breakdown = GetRecipeBreakdown(itemDB.fullRecipes);
                        foreach (Ingredient ingredient in breakdown.ingredients.Values)
                        {
                            int averagePrice = Price.getAveragePrice(itemID, worldID);
                            craftingCost += ingredient.quantity * averagePrice;
                        }
                    }
                    else
                    {
                        // Cannot be crafted
                        craftingCost = errorReturn;
                    }
                }


                if (craftingCost == 0)
                {
                    return errorReturn; //todo change to real value of crafting cost
                }
                else { return craftingCost; }
            }
            catch (Exception ex)
            {
                Log.Error("Failed to fetch the crafting cost for item_id: {item_id} world_id: {world_id}. {NewLine} Error message: {ex.Message}", itemID, worldID, ex.Message);
                return errorReturn;
            }

        }

        protected static RecipeIngredientBreakdown GetRecipeBreakdown(List<RecipeDB> fullRecipes)
        {
            if (fullRecipes == null || fullRecipes.Count == 0) { return null; }
            RecipeIngredientBreakdown breakdown = new RecipeIngredientBreakdown(fullRecipes);
            return breakdown;
        }
    }
}