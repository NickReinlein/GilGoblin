using GilGoblin.Database;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GilGoblin.Finance
{
    internal class RecipeIngredientBreakdown
    {
        public Dictionary<int, Ingredient> ingredients { get; set; } 
            = new Dictionary<int, Ingredient>();


        public RecipeIngredientBreakdown() { }
        public RecipeIngredientBreakdown(int itemID, Ingredient ingredient)
        {
            insert(itemID, ingredient);
        }
        public RecipeIngredientBreakdown(List<RecipeDB> list)
        {
            Add(list);
        }

        public void insert(int itemID, Ingredient ingredient)
        {
            if (ingredients.ContainsKey(itemID))
            {
                ingredients[itemID].quantity += ingredient.quantity;
            }
            else
            {
                ingredients.Add(itemID, ingredient);
            }
        }

        public void Add(List<RecipeDB> list)
        {
            if (list == null || list.Count == 0) { return; }
            foreach (RecipeDB recipe in list)
            {
                if (recipe != null)
                {
                    try
                    {
                        this.Add(recipe);
                    }
                    catch(Exception ex)
                    {
                        Log.Error("Failed to add recipe to the breakdown list: " +
                            "{recipeID},{NewLine} Error message: {ex.Message}",
                            recipe.recipe_id, ex.Message);
                    }
                }
            }
        }

        public void Add(RecipeDB recipe)
        {
            if (recipe == null)
            {
                Log.Error("Recipe is null and trying to add to the recipe ingredient breakdown.");

                return;
            }
            else
            {
                //Add all the ingredients in the recipe
                foreach (Ingredient ingredient in recipe.ingredients)
                {
                    try {
                        if (ItemInfoDB.IsCraftable(ingredient.item_id))
                        {
                            List<RecipeDB> ingredientBreakdownList
                                = ItemInfoDB.GetCraftingList(ingredient.item_id);
                    
                            this.Add(ingredientBreakdownList);
                    
                        }
                        else
                        {
                            this.insert(ingredient.item_id, ingredient);
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Failed to add ingredient to the breakdown list: " +
                            "{ingredient.item_id},{NewLine} Error message: {ex.Message}",
                            recipe.recipe_id, ex.Message);
                    }
                }
            }
        }

        public int getQuantity(int itemID)
        {
            int quantity = 0;
            Ingredient quantityIngredient = getIngredient(itemID);
            if (quantityIngredient != null)
            {
                quantity = quantityIngredient.quantity;
            }
            return quantity;
        }

        public Ingredient getIngredient(int itemID)
        {
            if (ingredients.ContainsKey(itemID))
            {
                return ingredients.ElementAt(itemID).Value;
            }
            else { return null; }
        }
    }
}
