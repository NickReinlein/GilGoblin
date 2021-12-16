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
    /// <summary>
    /// References the market data mapped to the database.
    /// ----------------
    /// Here we map the MarketData class to a header table of MarketData 
    /// (including average price for example) and the listings 
    /// to a body/secondary table with foreign keys to link it back to the 
    /// header table via the item_id and world_id
    /// </summary>
    internal class MarketDataDB : MarketData
    {
        //public ICollection<MarketListingWeb> listings { get; set; } = new List<MarketListingWeb>();
        public MarketDataDB() : base() { }
        public MarketDataDB(MarketData data) : base() 
        {
            listings = data.listings;
            this.last_updated = data.last_updated;
            this.average_Price = data.average_Price;
            this.item_id = data.item_id;
            this.world_id = data.world_id;
        }
        //public MarketDataDB(MarketDataDB db) : base()
        //{
        //    listings = null;
        //    this.last_updated = db.last_updated;
        //    this.average_Price = db.average_Price;
        //    this.item_id = db.item_id;
        //    this.world_id = db.world_id;
        //    foreach (MarketListingWeb listing in db.listings)
        //    {
        //        listings.Add(listing.ConvertToDB());
        //    }
        //}

        /////Here we map the market data coming from web API's to
        /////the database format using Entity Framework
        //public MarketDataDB(MarketDataWeb marketData)
        //{
        //    this.item_id = marketData.item_id;
        //    this.world_id = marketData.world_id;
        //    this.last_updated = DateTime.Now;
        //    this.average_Price = marketData.average_Price;
        //    foreach (MarketListingWeb listing in marketData.listings)
        //    {
        //        MarketListingDB listingDB = new MarketListingDB(listing);
        //        this.listings.Add(listingDB);
        //    }

        //}
    }
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
