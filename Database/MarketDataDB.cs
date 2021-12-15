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
        public int item_id { get; set; }
        public int world_id { get; set; }
        public DateTime last_updated { get; set; }
        public int average_Price { get; set; }

        public ICollection<MarketListingDB> listings { get; set; } = new List<MarketListingDB>();

        public MarketDataDB() { }

        public MarketDataDB(MarketData marketData)
        {
            this.item_id = marketData.item_id;
            this.world_id = marketData.world_id;
            this.last_updated = DateTime.Now;
            this.average_Price = marketData.average_Price;
            foreach (MarketListing listing in marketData.listings)
            {
                MarketListingDB listingDB = new MarketListingDB(listing);
                this.listings.Add(listingDB);
            }

        }
    }

    public class MarketListingDB
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]

        [ForeignKey("item_id")]
        [InverseProperty("MarketData")]
        public int item_id { get; set; }
        public int world_id { get; set; }
        public DateTime timestamp { get; set; }
        public int price { get; set; }
        public bool hq { get; set; }
        public int qty { get; set; }

        public MarketListingDB() {}
        public MarketListingDB(MarketListing listing)
        {
            this.item_id = listing.item_id;
            this.world_id = listing.world_id;
            this.timestamp = listing.timestamp;
            this.price = listing.price;
            this.hq = listing.hq;
            this.qty = listing.qty;
        }
    }
}
