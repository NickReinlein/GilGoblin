
using GilGoblin.Database;
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
        public float averagePrice { get; set; }
        public ICollection<MarketListingWeb> listings { get; set; } = new List<MarketListingWeb>();

        public static int listingsToRead = 20; //TODO increase for production use

        public float getPrice(bool update = false)
        {
            if (update || averagePrice == 0)
            {
                //Re-calculate the price and update
                CalculateAveragePrice(listings);
            }

            //Return what we do have regardless of updates
            return averagePrice;
        }

        [JsonConstructor]
        public MarketDataWeb(int itemID, int worldId, long lastUploadTime,
                               ICollection<MarketListingWeb> listings) : base()
        {
            if (itemID == 0 || worldId == 0 || lastUploadTime == 0 || listings == null || listings.Count == 0)
            {
                throw new ArgumentException("Incorrect/missing parameters/arguments coming from the web response.");
            }
            this.itemID = itemID;
            this.worldID = worldId;
            this.lastUpdated
                = GeneralFunctions.ConvertLongUnixMillisecondsToDateTime(lastUploadTime);

            this.listings = listings.OrderByDescending(i => i.timestamp).Take(listingsToRead).ToList();

            foreach (MarketListingWeb listing in this.listings)
            {
                listing.item_id = itemID;
                listing.world_id = worldId;
            }

            if (this.listings.Count > 0)
            {
                this.averagePrice = (int)getPrice();
            }
            else { this.averagePrice = 0; }

        }

        public static async Task<MarketDataWeb> FetchMarketData(int item_id, int world_id)
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = "https://universalis.app/api/" + world_id + "/" + item_id;
                var content = await client.GetAsync(url);

                //Deserialize from JSON to the object
                MarketDataWeb marketData = JsonConvert.DeserializeObject<MarketDataWeb>(content.Content.ReadAsStringAsync().Result);
                return marketData;
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
                foreach (int itemID in itemIDs)
                {
                    url += String.Concat(itemID + "%2C");
                }
                var content = await client.GetAsync(url);
                var json = content.Content.ReadAsStringAsync().Result;

                //Deserialize from JSON to the object
                MarketDataWebBulk bulkData = JsonConvert.DeserializeObject<MarketDataWebBulk>(json);
                if (bulkData != null)
                {
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