using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using GilGoblin.Finance;

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
                    content.Content.ReadAsStringAsync().Result );

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
                Market_Data market_Data = JsonConvert.DeserializeObject<Market_Data>(
                    content.Content.ReadAsStringAsync().Result );

                return market_Data.get_Price();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }
        }

     
    }
}


