using GilGoblin.Finance;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace GilGoblin.WebAPI
{
    /// <summary>
    /// Fetches market information such as market price 
    /// </summary>
    internal class Market
    {
        public static async Task<int> Get_Market_Price(int item_id, string world_name)
        {
            Console.WriteLine("Starting to fetch market price");
            try
            {
                HttpClient client = new HttpClient();
                string url = "https://universalis.app/api/history/" + world_name + "/" + item_id;
                var content = await client.GetAsync(url);

                //Deserialize & Cast from JSON to the object
                Market_Data market_Data = JsonConvert.DeserializeObject<Market_Data>(
                    content.Content.ReadAsStringAsync().Result);

                return market_Data.get_Price();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

        public static async Task<int> Get_Item_Info(int item_id)
        {
            Console.WriteLine("Starting to fetch cost");
            try
            {
                HttpClient client = new HttpClient();
                string url = "https://xivapi.com/Item/" + item_id;
                var content = await client.GetAsync(url);

                //Deserialize & Cast from JSON to the object
                Item_Info item_info = JsonConvert.DeserializeObject<Item_Info>(
                    content.Content.ReadAsStringAsync().Result);

                Console.WriteLine("Found the item info for " + item_info.description);

                Console.WriteLine("Found recipes: ");
                item_info.recipes.ForEach(
                    recipe => Console.WriteLine("Job: " + recipe.class_job_id + " / " +
                                                "Level req: " + recipe.level + " / " +
                                                "Item req: " + recipe.item_id));

                Console.WriteLine("Vendor price found of: " + item_info.vendor_price);

                return item_info.vendor_price;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }


    }
}


