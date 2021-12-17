
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using GilGoblin.Functions;
using System.Threading.Tasks;
using System.Net.Http;
using GilGoblin.WebAPI;
using GilGoblin.Finance;

namespace GilGoblin.WebAPI
{
    /// <summary>
    /// Here we map the market data coming via web API's.
    /// </summary>
    internal class MarketDataWeb : MarketData
    {
        public ICollection<MarketListingWeb> listings { get; set; } = new List<MarketListingWeb>();
        public static int listingsToRead = 20; //TODO increase for production use

        public int getPrice(bool update = false)
        {
            if (update || average_Price == 0)
            {
                //Re-calculate the price and update
                CalculateAveragePrice(listings);
            }

            //Return what we do have regardless of updates
            return average_Price;
        }

        [JsonConstructor]
        public MarketDataWeb(int itemID, int worldId, long lastUploadTime,
                               ICollection<MarketListingWeb> entries) : base()
        {
            this.item_id = itemID;
            this.world_id = worldId;
            this.last_updated 
                = General_Function.ConvertLongUnixMillisecondsToDateTime(lastUploadTime);

            this.listings = entries.OrderByDescending(i => i.timestamp).Take(listingsToRead).ToList();

            foreach (MarketListingWeb listing in listings)
            {
                listing.item_id=itemID;
                listing.world_id=worldId;
            }

            this.average_Price =  CalculateAveragePrice(this.listings);
        }

        public static async Task<MarketDataWeb> FetchMarketData(int item_id, int world_id)
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = "https://universalis.app/api/history/" + world_id + "/" + item_id;
                var content = await client.GetAsync(url);

                //Deserialize from JSON to the object
                MarketDataWeb market_Data = JsonConvert.DeserializeObject<MarketDataWeb>(content.Content.ReadAsStringAsync().Result);
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

    }


}