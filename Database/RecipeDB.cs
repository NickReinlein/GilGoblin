using GilGoblin.Finance;
using GilGoblin.WebAPI;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http;
using System.Threading.Tasks;

namespace GilGoblin.Database
{
    public class RecipeDB : Recipe
    {                
        public RecipeDB() : base() { }
        public RecipeDB(RecipeFullWeb web) : base()
        {
            icon_id = web.icon_id;
            CanHq = web.CanHq;
            CanQuickSynth = web.CanQuickSynth;
            target_item_id = web.target_item_id;
            recipe_id = web.recipe_id;
            result_quantity = web.result_quantity;
            if (web.ingredients != null)
            {
                this.ingredients = web.ingredients;
            }
        }

        public static RecipeDB FetchRecipe(int recipe_id)
        {            
          return new RecipeDB(RecipeFullWeb.FetchRecipe(recipe_id).GetAwaiter().GetResult());
        }
    }
}
