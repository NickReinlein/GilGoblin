using GilGoblin.pocos;
using Newtonsoft.Json;

namespace GilGoblin.web
{
    public class RecipeGateway : IRecipeGateway
    {
        public RecipeFullPoco GetRecipe(int recipeID) => new RecipeFullPoco();
        public RecipeFullPoco FetchRecipe(int recipeID) => new RecipeFullPoco();
        public IEnumerable<RecipeFullPoco> GetRecipesForItem(int itemID) => Array.Empty<RecipeFullPoco>();
        // public class RecipeFetcher //: IRecipeFetcher
        // {
        //     public async Task<RecipeFullPoco?> FetchRecipe(int recipe_id)
        //     {
        //         RecipeFullPoco? fullRecipe;
        //         try
        //         {
        //             HttpClient client = new HttpClient();
        //             string url = "https://xivapi.com/recipe/" + recipe_id;
        //             var content = await client.GetAsync(url);

        //             //Deserialize & Cast from JSON to the object
        //             fullRecipe = JsonConvert.DeserializeObject<RecipeFullPoco>(content.Content.ReadAsStringAsync().Result);
        //         }
        //         catch (Exception ex)
        //         {
        //             //Log.Error("Failed to convert recipe from JSON: {message}", ex.Message);
        //             return null;
        //         }
        //         return fullRecipe;
        //     }
        // }
    }
}