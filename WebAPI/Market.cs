using System;
using System.Net.Http;
using System.Threading.Tasks;
using Flurl.Http;
using Newtonsoft.Json;

namespace GilGoblin.WebAPI
{
    /// <summary>
    /// Fetches market information such as market price for the server
    /// </summary>
    internal class Market
    {
        //private string api_key = 
        public static int Get_Market_price(int item_id, string world_name)
        {
            //Task market_task = Fetch_Market_Price(item_id);

            Fetch_Market_Price(item_id, world_name);
            return 0;
        }
        public static async void Fetch_Market_Price(int item_id, string world_name)
        {
            int market_price = 0;
            Console.WriteLine("Starting to fetch");
            try
            {
                //https://universalis.app/api/history/Brynhildr/5114
                string url = "https://universalis.app/api/history/" + world_name + "/" + item_id;
                IFlurlResponse req = await url.GetJsonAsync();


                //dynamic item = JsonConvert.DeserializeObject(
                //    req.Content.ReadAsStringAsync().Result
                //);
                //Console.Write($"I.Lv {item.LevelItem} {item.Name_en}");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                Console.WriteLine("Returning now");
            }
        }
    }
}
