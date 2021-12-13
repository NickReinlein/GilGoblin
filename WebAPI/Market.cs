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
        public static async Task<MarketData> FetchMarketData(int item_id, string world_name)
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = "https://universalis.app/api/history/" + world_name + "/" + item_id;
                var content = await client.GetAsync(url);

                //Deserialize & Cast from JSON to the object
                MarketData market_Data = JsonConvert.DeserializeObject<MarketData>(
                    content.Content.ReadAsStringAsync().Result);
                foreach( MarketListing listing in market_Data.listings)
                {
                    listing.world_name = world_name;
                }
                return market_Data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to convert market data from JSON:" + ex.Message);
                return null;
            }
        }


        public static async Task<ItemInfo> FetchItemInfo(int item_id)
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

        public static MarketData GetMarketData(int item_id, string world_name)
        {
            return Market.FetchMarketData(item_id, world_name).GetAwaiter().GetResult();
            //Try with the database first, then if it fails we use the web API
            //MarketData marketData = Database.DatabaseAccess.GetMarketDataDB(item_id);
            //if (marketData == null)
            //{
            //    marketData = Market.FetchMarketData(item_id, world_name).GetAwaiter().GetResult();
            //}
            //return marketData;
        }

        public static ItemInfo GetItemInfo(int item_id)
        {
            //TODO: later this needs to be doneDatabase.DatabaseAccess.GetMarketDataDB
            return Market.FetchItemInfo(item_id).GetAwaiter().GetResult();

        }
    }
}


