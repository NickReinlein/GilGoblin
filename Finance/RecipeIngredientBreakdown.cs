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
            this.Add(itemID, ingredient);
        }
        public RecipeIngredientBreakdown(List<RecipeDB> list)
        {
            Add(list);
        }

        public void Add(List<RecipeDB> list)
        {
            if (list == null || list.Count == 0) { return; }
            foreach (RecipeDB recipe in list)
            {
                if (recipe != null)
                {
                    this.Add(recipe);
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
                    if (ItemInfoDB.IsCraftable(ingredient.item_id))
                    {
                        List<RecipeDB> ingredientBreakdownList
                            = ItemInfoDB.GetCraftingList(ingredient.item_id);
                    
                        this.Add(ingredientBreakdownList);
                    
                    }
                    else
                    {
                        this.Add(ingredient.item_id, ingredient);
                    }
                }
            }
        }


        public void Add(int itemID, Ingredient ingredient)
        {
            ingredients.Add(itemID, ingredient);
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
