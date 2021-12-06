using System;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;
using GilGoblin.Finance;

namespace GilGoblin.WebAPI
{
    /// <summary>
    /// Fetches market information such as market price for the server
    /// </summary>
    internal class Market
    {
        

        public static async Task<int> Get_Market_price(int item_id, string world_name)
        {
            int market_price = await Fetch_Market_Price(item_id, world_name);

            return market_price;
        }
        public static async Task<int> Fetch_Market_Price(int item_id, string world_name)
        {
            Console.WriteLine("Starting to fetch");
            try
            {
                HttpClient client = new HttpClient();
                //Test url should be:
                //https://universalis.app/api/history/Brynhildr/5114                
                string url = "https://universalis.app/api/history/" + world_name + "/" + item_id;


                var content = await client.GetAsync(url);//.ConfigureAwait(false);                                
                //Deserialize & Cast from JSON to the object
                Market_Data market_Data = JsonConvert.DeserializeObject<Market_Data>(
                    content.Content.ReadAsStringAsync().Result );

                //var obj_for_type = JObject.Parse(item);
                //var jsonType = (String)obj_for_type["_type"];
                //Console.Write($"I.Lv {item.LevelItem} {item.Name_en}");

                return 0 ;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }
    }
}
