
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using GilGoblin.Functions;

namespace GilGoblin.Finance
{
    public class MarketListing
    {
        public int Id { get; set; }                
        public bool hq { get; set; }
        public int price { get; set; }
        public int qty { get; set; }
        public DateTime timestamp { get; set; }

        [JsonIgnore]
        public int world_id { get; set; }        
        public int item_id { get; set; }

        [JsonConstructor]
        public MarketListing(int pricePerUnit, int quantity, long timestamp,
                             bool hq)
        {
            this.hq = hq;
            this.price = pricePerUnit;
            this.qty = quantity;
            this.hq = hq;
            this.timestamp = General_Function.ConvertLongUnixSecondsToDateTime(timestamp);            
         }
    }

    public class MarketData
    {
        public int item_id { get; set; }
        public int world_id { get; set; }
        public DateTime last_updated { get; set; }
        public int average_Price { get; set; }

        public ICollection<MarketListing> listings { get; set; }

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


        [JsonConstructor]
        public MarketData(int itemID, int worldId, long lastUploadTime,
                               ICollection<MarketListing> entries)
        {
            this.item_id = itemID;
            this.world_id = worldId;
            this.last_updated 
                = General_Function.ConvertLongUnixMillisecondsToDateTime(lastUploadTime);

            this.listings = entries.OrderByDescending(i => i.timestamp).Take(listingsToRead).ToList();

            foreach (MarketListing listing in listings)
            {
                listing.item_id=itemID;
                listing.world_id=world_id;
            }

            this.average_Price = CalculateAveragePrice();
        }

    }
}