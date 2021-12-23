using GilGoblin.Database;
using GilGoblin.Finance;
using GilGoblin.Functions;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http;
using System.Threading.Tasks;

namespace GilGoblin.WebAPI
{
    public class RecipeFullWeb : Recipe
    {        
        public RecipeFullWeb() : base() { }

        [JsonConstructor]
        public RecipeFullWeb(bool CanQuickSynth, bool CanHq, int ItemResultTargetID,
            int ID, int IconID, int AmountResult,
            int AmountIngredient0,
            int AmountIngredient1,
            int AmountIngredient2,
            int AmountIngredient3,
            int AmountIngredient4,
            int AmountIngredient5,
            int AmountIngredient6,
            int AmountIngredient7,
            int AmountIngredient8,
            int AmountIngredient9,
            int ItemIngredient0TargetID,
            int ItemIngredient1TargetID,
            int ItemIngredient2TargetID,
            int ItemIngredient3TargetID,
            int ItemIngredient4TargetID,
            int ItemIngredient5TargetID,
            int ItemIngredient6TargetID,
            int ItemIngredient7TargetID,
            int ItemIngredient8TargetID,
            int ItemIngredient9TargetID) : base()
        {
            this.CanHq = CanHq;
            this.icon_id = IconID;
            this.target_item_id = ItemResultTargetID;
            this.recipe_id = ID;
            this.result_quantity = AmountResult;
            this.CanQuickSynth = CanQuickSynth;
            if (AmountIngredient0 > 0)
            {
                ingredients.Add(new Ingredient(ItemIngredient0TargetID, AmountIngredient0));
            }
            if (AmountIngredient1 > 0)
            {
                ingredients.Add(new Ingredient(ItemIngredient1TargetID, AmountIngredient1));
            }
            if (AmountIngredient2 > 0)
            {
                ingredients.Add(new Ingredient(ItemIngredient2TargetID, AmountIngredient2));
            }
            if (AmountIngredient3 > 0)
            {
                ingredients.Add(new Ingredient(ItemIngredient3TargetID, AmountIngredient3));
            }
            if (AmountIngredient4 > 0)
            {
                ingredients.Add(new Ingredient(ItemIngredient4TargetID, AmountIngredient4));
            }
            if (AmountIngredient5 > 0)
            {
                ingredients.Add(new Ingredient(ItemIngredient5TargetID, AmountIngredient5));
            }
            if (AmountIngredient6 > 0)
            {
                ingredients.Add(new Ingredient(ItemIngredient6TargetID, AmountIngredient6));
            }
            if (AmountIngredient7 > 0)
            {
                ingredients.Add(new Ingredient(ItemIngredient7TargetID, AmountIngredient7));
            }
            if (AmountIngredient8 > 0)
            {
                ingredients.Add(new Ingredient(ItemIngredient8TargetID, AmountIngredient8));
            }
            if (AmountIngredient9 > 0)
            {
                ingredients.Add(new Ingredient(ItemIngredient9TargetID, AmountIngredient9));
            }

        }

        public static async Task<RecipeFullWeb> FetchRecipe(int recipe_id)
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = "https://xivapi.com/recipe/" + recipe_id;
                var content = await client.GetAsync(url);

                //Deserialize & Cast from JSON to the object
                RecipeFullWeb recipe = JsonConvert.DeserializeObject<RecipeFullWeb>(content.Content.ReadAsStringAsync().Result);
                return recipe;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to convert recipe from JSON: {message}", ex.Message);
                return null;
            }
        }

        public RecipeDB convertToDB()
        {
            RecipeDB db = new RecipeDB();
            db.recipe_id = this.recipe_id;
            db.icon_id = this.icon_id;            
            db.target_item_id = this.target_item_id;
            db.recipe_id = this.recipe_id;
            db.result_quantity = this.result_quantity;
            db.CanHq = this.CanHq;
            db.CanQuickSynth = this.CanQuickSynth;
            return db;
        }
    }
}