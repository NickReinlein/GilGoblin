using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GilGoblin.Finance
{
    internal class Market_Listing
    {
        public bool hq { get; set; }
        public int price { get; set; }
        public int qty { get; set; }
        public  DateTime timestamp { get; set; }

        public Market_Listing(bool hq, int pricePerUnit, int quantity,
                              long timestamp)
        {
            this.hq = hq;
            this.price = pricePerUnit;
            this.qty = quantity;
            this.timestamp = DateTime.FromBinary(timestamp);
        }
    }

    internal class Market_Data
    {
        public int item_id { get; set; }
        public string world_name { get; set; }
        public string item_name { get; set; }
        public List<Market_Listing> current_listings{ get; set; }
        public DateTime last_updated { get; set; }

        public static int listingsToRead = 100;

    /// <summary>
    /// Constructor for JSON de-serialization; may add more constructors later
    /// </summary>
    /// <param name="item_id">The item's ID number</param>
    /// <param name="world_name">The world name</param>
    /// <param name="item_name">Optional: Item name</param>
    /// <param name="current_listings">Optional: current listings on the marketboard</param>
        public Market_Data(int itemID, string worldName,
                           string item_name, long lastUploadTime, 
                           List<Market_Listing> entries)
        {
            this.item_id = itemID;
            this.world_name = worldName;
            this.item_name = item_name;
            this.last_updated = DateTime.FromBinary(lastUploadTime);
            this.current_listings = 
                entries.OrderByDescending(i => i.timestamp).Take(listingsToRead).ToList();
        }
    }
}
