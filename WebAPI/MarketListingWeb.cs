
using GilGoblin.Database;
using GilGoblin.Finance;
using GilGoblin.Functions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace GilGoblin.WebAPI
{
    internal class MarketListingWeb : MarketListing
    {
        public int item_id { get; set; }
        public int world_id { get; set; }
        public DateTime timestamp { get; set; }
        public MarketListingWeb() { }

        [JsonConstructor]
        public MarketListingWeb(int pricePerUnit, int quantity, long timestamp, bool hq) : base()
        {
            this.hq = hq;
            this.price = pricePerUnit;
            this.qty = quantity;
            this.hq = hq;
            //this.timestamp = General_Function.ConvertLongUnixSecondsToDateTime(timestamp);
            this.timestamp = General_Function.ConvertLongUnixMillisecondsToDateTime(timestamp);
        }

        public MarketListingDB ConvertToDB()
        {
            MarketListingDB db = new MarketListingDB();
            db.item_id = item_id;
            db.price = price;
            db.qty = qty;
            db.hq = hq;
            db.timestamp = timestamp;
            db.world_id = world_id;
            return db;
        }

        public static ICollection<MarketListingDB> ConvertWebListingsToDB(ICollection<MarketListingWeb> webListings)
        {
            ICollection<MarketListingDB> dbListings = new List<MarketListingDB>();
            foreach (MarketListingWeb web in webListings)
            {
                dbListings.Add(web.ConvertToDB());
            }
            return dbListings;
        }
    }
}