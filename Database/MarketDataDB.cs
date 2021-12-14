using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GilGoblin.Finance;
using GilGoblin.WebAPI;

namespace GilGoblin.Database
{
    public class MarketDataDB
    {
        [Key, Column(Order=0)]
        public int item_id { get; set; }
        [Key, Column(Order = 1)]
        public string world_name { get; }
        public DateTime last_updated { get; set; }
        public int average_Price { get; set; }

        public MarketDataDB() { }

        public MarketDataDB(MarketData marketData)
        {
            this.item_id = marketData.item_id;
            this.world_name = marketData.world_name;
            this.last_updated = marketData.last_updated;            
            this.average_Price = marketData.average_Price;
        }
    }

    public class MarketListingDB
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public int item_id { get; set; }
        public string world_name { get; set;  }
        public DateTime timestamp { get; set; }
        public int price { get; set; }
        public bool hq { get; set; }
        public int qty { get; set; }

        public MarketListingDB() {}
        public MarketListingDB(MarketListing listing)
        {
            Random random = new Random();
            this.Id = random.Next();
            this.item_id = listing.item_id;
            this.world_name = listing.world_name;
            this.timestamp = listing.timestamp;
            this.price = listing.price;
            this.hq = listing.hq;
            this.qty = listing.qty;
        }
    }
}
