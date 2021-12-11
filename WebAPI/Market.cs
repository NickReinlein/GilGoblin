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
        public static async Task<MarketData> Get_Market_Data(int item_id, string world_name)
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = "https://universalis.app/api/history/" + world_name + "/" + item_id;
                var content = await client.GetAsync(url);

                //Deserialize & Cast from JSON to the object
                MarketData market_Data = JsonConvert.DeserializeObject<MarketData>(
                    content.Content.ReadAsStringAsync().Result);
                return market_Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to convert market data from JSON:" + ex.Message);
                return null;
            }
        }


        public static async Task<ItemInfo> Get_Item_Info(int item_id)
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = "https://xivapi.com/Item/" + item_id;
                var content = await client.GetAsync(url);

                //Deserialize & Cast from JSON to the object
                ItemInfo item_info = JsonConvert.DeserializeObject<ItemInfo>(
                    content.Content.ReadAsStringAsync().Result);
                return item_info;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to convert item info from JSON:" + ex.Message);
                return null;
            }
        }
    }
}


