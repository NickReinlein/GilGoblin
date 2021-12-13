
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GilGoblin.Functions;

namespace GilGoblin.Finance
{
    internal class MarketListing
    {
        public int item_id { get; private set; }
        public bool hq { get; set; }
        public int price { get; set; }
        public int qty { get; set; }
        public DateTime timestamp { get; set; }
        
        //World_name is set seprately as the API does not provide it
        //but it's not an issue since we have it when we call the API 
        [JsonIgnore]
        public string world_name { get; set; }

        public MarketListing(int item_id, bool hq, int pricePerUnit, 
                             int quantity, long timestamp)
        {
            this.item_id = item_id;
            this.hq = hq;
            this.price = pricePerUnit;
            this.qty = quantity;
            this.timestamp = DateTime.FromBinary(timestamp);
        }
    }

    internal class MarketData
    {
        public int item_id { get; private set; }
        public string world_name { get; private set; }
        public DateTime last_updated { get; set; }
        public int average_Price { get; set; }

        public List<MarketListing> listings { get; set; }

        public static int listingsToRead = 20; //TODO increase for production use

        public int getPrice(bool update = false)
        {
            if (update || average_Price == 0)
            {
                //Re-calculate the price and update
                CalculateAveragePrice(true);
            }

            //Return what we do have regardless of updates
            return average_Price;
        }

        public int CalculateAveragePrice(bool update = false)
        {
            int average_Price = 0;
            uint total_Qty = 0;
            ulong total_Gil = 0;
            foreach (MarketListing listing in this.listings)
            {
                total_Qty += ((uint)listing.qty);
                total_Gil += ((ulong)(listing.price * listing.qty));
            }
            if (total_Qty > 0)
            {
                float average_Price_f = total_Gil / total_Qty;
                average_Price = (int)Math.Round(average_Price_f);
            }
            return average_Price;
        }


        /// <summary>
        /// Constructor for JSON de-serialization; may add more constructors later
        /// </summary>
        /// <param name="item_id">The item's ID number</param>
        /// <param name="world_name">The world name</param>
        /// <param name="item_name">Optional: Item name</param>
        /// <param name="current_listings">Optional: current listings on the marketboard</param>
        [JsonConstructor]
        public MarketData(int itemID, string worldName, long lastUploadTime,
                               List<MarketListing> entries)
        {
            this.item_id = itemID;
            this.world_name = worldName;
            this.last_updated 
                = General_Function.ConvertLongToDateTime(lastUploadTime);

            this.listings =
                entries.OrderByDescending(i => i.timestamp).Take(listingsToRead).ToList();

            this.average_Price = CalculateAveragePrice();
        }

        public MarketData(int itemID, string worldName, long lastUploadTime,
                               List<MarketListing> entries, int averagePrice)
        {
            this.item_id = itemID;
            this.world_name = worldName;
            this.last_updated = Functions.General_Function.ConvertLongToDateTime(lastUploadTime);

            //Order listings by most recent to older & take the most recent X listings to save
            this.listings =
                entries.OrderByDescending(i => i.timestamp).Take(listingsToRead).ToList();

            this.average_Price = average_Price;
        }

    }
}