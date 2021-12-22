
using GilGoblin.Finance;
using GilGoblin.Functions;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GilGoblin.WebAPI
{
    /// <summary>
    /// Here we map the market data coming via web API's.
    /// </summary>
    internal class MarketDataWeb : MarketData
    {
        public ICollection<MarketListingWeb> listings { get; set; } = new List<MarketListingWeb>();
        public static int listingsToRead = 20; //TODO increase for production use

        public ICollection<APIRecipe> recipes { get; set; } = new List<APIRecipe>();

        public int getPrice(bool update = false)
        {
            if (update || average_price == 0)
            {
                //Re-calculate the price and update
                CalculateAveragePrice(listings);
            }

            //Return what we do have regardless of updates
            return average_price;
        }

        [JsonConstructor]
        public MarketDataWeb(int itemID, int worldId, long lastUploadTime,
                               ICollection<MarketListingWeb> listings) : base()
        {
            if (itemID == 0 || worldId == 0 || lastUploadTime == 0 || listings == null || listings.Count == 0)
            {
                throw new ArgumentException("Incorrect/missing parameters/arguments coming from the web response.");
            }
            this.item_id = itemID;
            this.world_id = worldId;
            this.last_updated
                = GeneralFunctions.ConvertLongUnixMillisecondsToDateTime(lastUploadTime);

            this.listings = listings.OrderByDescending(i => i.timestamp).Take(listingsToRead).ToList();

            foreach (MarketListingWeb listing in this.listings)
            {
                listing.item_id = itemID;
                listing.world_id = worldId;
            }

            this.average_price = CalculateAveragePrice(this.listings);
        }

        public static async Task<MarketDataWeb> FetchMarketData(int item_id, int world_id)
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = "https://universalis.app/api/" + world_id + "/" + item_id;
                var content = await client.GetAsync(url);

                //Deserialize from JSON to the object
                MarketDataWeb market_Data = JsonConvert.DeserializeObject<MarketDataWeb>(content.Content.ReadAsStringAsync().Result);
                return market_Data;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to convert market data from JSON:" + ex.Message);
                return null;
            }
        }

        public static async Task<List<MarketDataWeb>> FetchMarketDataBulk(List<int> itemIDs, int world_id)
        {
            try
            {
                List<MarketDataWeb> list = new List<MarketDataWeb>();
                HttpClient client = new HttpClient();
                //https://universalis.app/api/34/5114%2C5106%2C5057
                string url = String.Concat("https://universalis.app/api/", world_id, "/");
                foreach (int itemID in itemIDs) {
                    url += String.Concat(itemID + "%2C");
                }
                var content = await client.GetAsync(url);
                var json = content.Content.ReadAsStringAsync().Result;

                //Deserialize from JSON to the object
                MarketDataWebBulk bulkData = JsonConvert.DeserializeObject<MarketDataWebBulk>(json);
                if (bulkData != null) { 
                    list = bulkData.items; 
                }
                return list;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to convert market data from JSON:" + ex.Message);
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
                Log.Error("Failed to convert item info from JSON:" + ex.Message);
                return null;
            }
        }

    }

    internal class MarketDataWebBulk
    {
        public List<int> itemIDs { get; set; } = new List<int>();
        public List<MarketDataWeb> items { get; set; } = new List<MarketDataWeb>();

        [JsonConstructor]
        public MarketDataWebBulk(ICollection<int> itemIDs, 
                                 ICollection<MarketDataWeb> items): base()
        {
            if (itemIDs != null) { this.itemIDs = new List<int>(itemIDs); }
            if (items != null) { this.items = new List<MarketDataWeb>(items); }
        }
    }

}