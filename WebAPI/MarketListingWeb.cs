
using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using GilGoblin.Functions;
using GilGoblin.Finance;
using GilGoblin.Database;

namespace GilGoblin.WebAPI
{
    internal class MarketListingWeb : MarketListing
    {
        public int item_id { get; set; }
        public int world_id { get; set; }
        public DateTime timestamp { get; set; }
        public MarketListingWeb(){}

        [JsonConstructor]
        public MarketListingWeb(int pricePerUnit, int quantity, long timestamp, bool hq)
        {
            this.hq = hq;
            this.price = pricePerUnit;
            this.qty = quantity;
            this.hq = hq;
            this.timestamp = General_Function.ConvertLongUnixSecondsToDateTime(timestamp);            
         }

        //public MarketListingDB ConvertToDB()
        //{
        //    MarketListingDB db = new MarketListingDB();
        //    db.item_id = item_id;
        //    db.price = price;
        //    db.qty = qty;
        //    db.timestamp = timestamp;
        //    return db;
        //}
    }
}