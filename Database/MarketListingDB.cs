using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GilGoblin.Finance;
using GilGoblin.WebAPI;

namespace GilGoblin.Database
{
    /// <summary>
    /// Database format for a single market listing
    /// </summary>
    internal class MarketListingDB : MarketListing
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [ForeignKey("item_id")]
        [InverseProperty("MarketDataDB")]
        public int item_id { get; set; }

        [ForeignKey("world_id")]
        [InverseProperty("MarketDataDB")]
        public int world_id { get; set; }
        public DateTime timestamp { get; set; }

        public MarketListingDB() {}
        public MarketListingDB(MarketListingWeb listing)
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
