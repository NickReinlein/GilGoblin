using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http;
using System.Threading.Tasks;

namespace GilGoblin.POCOs
{
    public class RecipeFullPoco
    {        
        public int recipeID { get; set; }
        public int targetItemID { get; set; }
        public int iconId { get; set; }
        public int resultQuantity { get; set; }
        public bool canHq { get; set; }
        public bool canQuickSynth { get; set; }
        public ICollection<IngredientPoco> ingredients { get; set; }
            = new List<IngredientPoco>();
        public RecipeFullPoco() : base() { }

        [JsonConstructor]
        public RecipeFullPoco(bool CanQuickSynth, bool CanHq, int ItemResultTargetID,
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
            this.canHq = CanHq;
            this.iconId = IconID;
            this.targetItemID = ItemResultTargetID;
            this.recipeID = ID;
            this.resultQuantity = AmountResult;
            this.canQuickSynth = CanQuickSynth;
            if (AmountIngredient0 > 0)
            {
                ingredients.Add(new IngredientPoco(ItemIngredient0TargetID, AmountIngredient0,this.recipeID));
            }
            if (AmountIngredient1 > 0)
            {
                ingredients.Add(new IngredientPoco(ItemIngredient1TargetID, AmountIngredient1, this.recipeID));
            }
            if (AmountIngredient2 > 0)
            {
                ingredients.Add(new IngredientPoco(ItemIngredient2TargetID, AmountIngredient2, this.recipeID));
            }
            if (AmountIngredient3 > 0)
            {
                ingredients.Add(new IngredientPoco(ItemIngredient3TargetID, AmountIngredient3, this.recipeID));
            }
            if (AmountIngredient4 > 0)
            {
                ingredients.Add(new IngredientPoco(ItemIngredient4TargetID, AmountIngredient4, this.recipeID));
            }
            if (AmountIngredient5 > 0)
            {
                ingredients.Add(new IngredientPoco(ItemIngredient5TargetID, AmountIngredient5, this.recipeID));
            }
            if (AmountIngredient6 > 0)
            {
                ingredients.Add(new IngredientPoco(ItemIngredient6TargetID, AmountIngredient6, this.recipeID));
            }
            if (AmountIngredient7 > 0)
            {
                ingredients.Add(new IngredientPoco(ItemIngredient7TargetID, AmountIngredient7, this.recipeID));
            }
            if (AmountIngredient8 > 0)
            {
                ingredients.Add(new IngredientPoco(ItemIngredient8TargetID, AmountIngredient8, this.recipeID));
            }
            if (AmountIngredient9 > 0)
            {
                ingredients.Add(new IngredientPoco(ItemIngredient9TargetID, AmountIngredient9, this.recipeID));
            }

        }

        public static async Task<RecipeFullPoco?> FetchRecipe(int recipe_id)
        {
            RecipeFullPoco? fullRecipe;
            try
            {
                HttpClient client = new HttpClient();
                string url = "https://xivapi.com/recipe/" + recipe_id;
                var content = await client.GetAsync(url);

                //Deserialize & Cast from JSON to the object
                fullRecipe = JsonConvert.DeserializeObject<RecipeFullPoco>(content.Content.ReadAsStringAsync().Result);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to convert recipe from JSON: {message}", ex.Message);
                return null;
            }

            return fullRecipe;
        }
    }
}