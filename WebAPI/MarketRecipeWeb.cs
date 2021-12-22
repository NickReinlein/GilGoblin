
using GilGoblin.Database;
using GilGoblin.Finance;
using GilGoblin.Functions;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GilGoblin.WebAPI
{

    public class MarketRecipeWeb
    {
        public int target_item_id { get; set; }
        public int icon_id { get; set; }
        public int recipe_id { get; set; }
        public int result_quantity { get; set; }
        public bool CanHq { get; set; }
        public bool CanQuickSynth { get; set; }

        //Dictionary representing item ID & quantity
        Dictionary<int, int> ingredients { get; set; } = new Dictionary<int, int>();

        //public MarketRecipeWeb() { }

        [JsonConstructor]
        public MarketRecipeWeb(bool CanQuickSynth, bool CanHq, int ItemResultTargetID,
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
        int ItemIngredient8TargetID,
        int ItemIngredient9TargetID)
        {
            this.CanHq = CanHq;
            this.icon_id = IconID;
            this.target_item_id = ItemResultTargetID;
            this.recipe_id = ID;
            this.result_quantity = AmountResult;
            this.CanQuickSynth = CanQuickSynth;
            if (AmountIngredient0 > 0)
            {
                ingredients.Add(ItemIngredient0TargetID, AmountIngredient0);
            }
            if (AmountIngredient1 > 0)
            {
                ingredients.Add(ItemIngredient1TargetID, AmountIngredient1);
            }
        }

        public static async Task<MarketRecipeWeb> FetchRecipe(int recipe_id)
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = "https://xivapi.com/recipe/" + recipe_id;
                var content = await client.GetAsync(url);

                //Deserialize & Cast from JSON to the object
                MarketRecipeWeb recipe = JsonConvert.DeserializeObject<MarketRecipeWeb>(content.Content.ReadAsStringAsync().Result);
                return recipe;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to convert recipe from JSON: {ex.Message}");
                return null;
            }
        }

        //public MarketListingDB ConvertToDB()
        //{
        //    MarketListingDB db = new MarketListingDB();
        //    return db;
        //}

        //public static ICollection<MarketListingDB> ConvertWebListingsToDB(ICollection<MarketRecipeWeb> webListings)
        //{
        //    ICollection<MarketListingDB> dbListings = new List<MarketListingDB>();
        //    foreach (MarketRecipeWeb web in webListings)
        //    {
        //        dbListings.Add(web.ConvertToDB());
        //    }
        //    return dbListings;
        //}
    }
}