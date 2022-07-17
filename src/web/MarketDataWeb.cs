using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace GilGoblin.POCOs
{
    internal class MarketDataPoco
    {
        public int itemId { get; set; }
        public int worldId { get; set; }
        public float averagePrice { get; set; }
        public DateTime lastUpdated { get; set; }
        public ICollection<MarketListingPoco> listings { get; set; } = new List<MarketListingPoco>();
        public static int listingsToRead = 20;

        [JsonConstructor]
        public MarketDataPoco(int itemId, int worldId, long lastUploadTime,
                               ICollection<MarketListingPoco> listings) : base()
        {
            if (itemId == 0 || worldId == 0 || lastUploadTime == 0)
            {
                const string MessageTemplate 
                    = "Incorrect/missing parameters/arguments coming from the web response. ItemID:{itemID}, worldID:{worldID},lastUpLoadTime:{last}";
                Log.Error(MessageTemplate, itemId, this.worldId, lastUploadTime);
            }
            
            this.itemId = itemId;
            this.worldId = worldId;
            this.lastUpdated = GeneralFunctions.ConvertLongUnixMillisecondsToDateTime(lastUploadTime);

            this.listings = listings.OrderByDescending(i => i.timestamp).Take(listingsToRead).ToList();

            foreach (MarketListingPoco listing in this.listings)
            {
                listing.itemID = itemId;
                listing.worldId = worldId;
            }

            this.averagePrice = (int)CalculateAveragePriceFromListings();

        }

        public static async Task<MarketDataPoco?> FetchMarketData(int itemID, int worldID)
        {
            try
            {
                HttpClient client = new HttpClient();
                string url = "https://universalis.app/api/" + worldID + "/" + itemID;
                var content = await client.GetAsync(url);

                //Deserialize from JSON to the object
                MarketDataPoco? marketData = JsonConvert.DeserializeObject<MarketDataPoco>(content.Content.ReadAsStringAsync().Result);
                return marketData;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to convert market data from JSON:" + ex.Message);
                return null;
            }
        }

        public static async Task<List<MarketDataPoco>?> FetchMarketDataBulk(List<int> itemIDs, int world_id)
        {
            try
            {
                List<MarketDataPoco> list = new List<MarketDataPoco>();
                HttpClient client = new HttpClient();
                //example: https://universalis.app/api/34/5114%2C5106%2C5057
                string url = String.Concat("https://universalis.app/api/", world_id, "/");
                foreach (int itemID in itemIDs)
                {
                    url += String.Concat(itemID + "%2C");
                }
                var content = await client.GetAsync(url);
                var json = content.Content.ReadAsStringAsync().Result;

                //Deserialize from JSON to the object
                MarketDataBulkPoco? bulkData = JsonConvert.DeserializeObject<MarketDataBulkPoco>(json);
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

        private int CalculateAveragePriceFromListings()
        {
            if (listings.Count == 0) { return 0; }
            int average_Price = 0;
            uint total_Qty = 0;
            ulong total_Gil = 0;
            try
            {
                foreach (MarketListingPoco listing in listings)
                {
                    if ((listing.quantity > 1 && listing.price > 200000) ||
                        (listing.quantity == 1 && listing.price > 400000))
                    {
                        continue; //Exclude crazy prices
                    }

                    total_Qty += ((uint)listing.quantity);
                    total_Gil += ((ulong)(listing.price * listing.quantity));
                }
                if (total_Qty > 0)
                {
                    float average_Price_f = total_Gil / total_Qty;
                    average_Price = (int)Math.Round(average_Price_f);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error calculating average price for item {itemID} world {worldID}. Message: {message}", this.itemId, this.worldId, ex.Message);
                average_Price = 0;
            }
            return average_Price;
        }            
    }

    internal class MarketDataBulkPoco
    {
        public List<int> itemIDs { get; set; } = new List<int>();
        public List<MarketDataPoco> items { get; set; } = new List<MarketDataPoco>();

        [JsonConstructor]
        public MarketDataBulkPoco(ICollection<int> itemIDs, 
                                 ICollection<MarketDataPoco> items): base()
        {
            if (itemIDs != null) { this.itemIDs = new List<int>(itemIDs); }
            if (items != null) { this.items = new List<MarketDataPoco>(items); }
        }
    }

}