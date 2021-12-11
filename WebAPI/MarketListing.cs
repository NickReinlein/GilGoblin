
using System;
using System.Collections.Generic;
using System.Linq;

namespace GilGoblin.Finance
{
    internal class MarketListing
    {
        public bool hq { get; set; }
        public int price { get; set; }
        public int qty { get; set; }
        public DateTime timestamp { get; set; }

        public MarketListing(bool hq, int pricePerUnit, int quantity,
                              long timestamp)
        {
            this.hq = hq;
            this.price = pricePerUnit;
            this.qty = quantity;
            this.timestamp = DateTime.FromBinary(timestamp);
        }
    }

    internal class MarketData
    {
        public int item_id { get; }
        public string world_name { get; }
        public List<MarketListing> current_listings { get; set; }
        public DateTime last_updated { get; set; }
        public int average_Price = 0;

        public static int listingsToRead = 20; //TODO increase for production use

        public int get_Price(bool update = false)
        {
            if (update || average_Price == 0)
            {
                //Re-calculate the price and update
                Calculate_Average_Price(true);
            }

            //Return what we do have regardless of updates
            return average_Price;
        }

        public int Calculate_Average_Price(bool update = false)
        {
            int average_Price = 0;
            uint total_Qty = 0;
            ulong total_Gil = 0;
            foreach (MarketListing listing in this.current_listings)
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
        public MarketData(int itemID, string worldName, long lastUploadTime,
                               List<MarketListing> entries)
        {
            this.item_id = itemID;
            this.world_name = worldName;
            this.last_updated = Functions.General_Function.Conver_Long_To_DateTime(lastUploadTime);

            this.current_listings =
                entries.OrderByDescending(i => i.timestamp).Take(listingsToRead).ToList();

            this.average_Price = Calculate_Average_Price();
        }

    }
}
