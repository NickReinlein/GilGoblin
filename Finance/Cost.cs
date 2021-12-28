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

                if (itemDB != null && itemDB.fullRecipes.Count > 0)
                {
                    DatabaseAccess.SaveRecipes(itemDB.fullRecipes).GetAwaiter().GetResult();
                    RecipeList recipeList = createTree(itemDB, worldID);
                    foreach (RecipeNode<RecipeDB> recipe in recipeList.list)
                    {
                        // Calculate cost of the recipeNode
                        int thisCost = GetRecipeCost(recipe._value, worldID);
                        if (thisCost > craftingCost)
                        {
                            craftingCost = thisCost;
                        }
                        else
                        {
                            craftingCost = thisCost;
                        }
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

        protected static int GetRecipeCost(RecipeDB recipe, int worldID)
        {
            int cost = 0;
            int errorReturn = 9999999;
            RecipeNode<RecipeDB> recipeNode = createNode(recipe, worldID);
            if (recipeNode != null)
            {
                
            }
            return errorReturn;
            
        }

        /// <summary>
        /// Each full crafting reicpe will have a RecipeNode object
        /// to represent the tree. The returned list is a list of trees
        /// with each tree/RecipeNode representing
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        protected static RecipeList createTree(ItemDB item, int worldID)
        {
            if (item == null || item.fullRecipes.Count == 0)
            {
                return null;
            }
            return createTree(item.fullRecipes, worldID);
        }

        protected static RecipeList createTree(List<RecipeDB> list, int worldID)
        {
            RecipeList finalRecipeList = new RecipeList();
            foreach (RecipeDB thisRecipe in list)
            {
                RecipeNode<RecipeDB> thisRecipeNode = createNode(thisRecipe, worldID);
                if (thisRecipeNode != null)
                {
                    finalRecipeList.list.Add(thisRecipeNode);
                }
            }

            return finalRecipeList;
        }

        protected static RecipeNode<RecipeDB> createNode(RecipeDB recipe, int worldID)
        {
            if (recipe == null)
            {
                Log.Error("Error creating crafting tree for recipe {recipeID} with target item {targetItemID}.", recipe.recipe_id, recipe.target_item_id);
                return null;
            }
            RecipeNode<RecipeDB> recipeNode = new RecipeNode<RecipeDB>(recipe);
            foreach (Ingredient ingredient in recipe.ingredients)
            {
                RecipeNode<RecipeDB> ingredientRecipe = createNode(ingredient, worldID);
                recipeNode.InsertChild(ingredientRecipe.Parent, ingredientRecipe._value);
            }

            return recipeNode;
        }

        protected static RecipeNode<RecipeDB> createNode(Ingredient ingredient, int worldID)
        {
            if (ingredient == null)
            {
                Log.Error("Error creating crafting tree for ingredients.");
                return null;
            }

            try
            {
                ItemDB thisIngredientItem = ItemDB.GetItemDBSingle(ingredient.item_id, worldID);
                var test = createTree(thisIngredientItem, worldID);
                return null;
            }
            catch (Exception ex)
            {
                Log.Error("Error creating crafting tree for ingredients. Message: {message}", ex.Message);
                return null;
            }

        }
    }
}