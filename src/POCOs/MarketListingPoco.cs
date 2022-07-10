using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;

namespace GilGoblin.POCOs
{
    internal class MarketListingPoco
    {
        public int itemID { get; set; }
        public int worldId { get; set; }
        public DateTime timestamp { get; set; }
        public bool hq { get; set; }
        public int price { get; set; }
        public int quantity { get; set; }

        public MarketListingPoco() {  }        
        public MarketListingPoco(int pricePerUnit, int quantity, long lastReviewTime, bool hq) : base()
        {
            this.hq = hq;
            this.price = pricePerUnit;
            this.quantity = quantity;
            this.hq = hq;
            if (timestamp.Equals(0))
            {
                Log.Error("Empty timestamp for web listing.");
            }
            else
            {
                try
                {
                    this.timestamp = GeneralFunctions.ConvertLongUnixSecondsToDateTime(lastReviewTime);
                }
                catch (Exception ex)
                {
                    Log.Error("Error converting web listing's timestamp of:  {this.timestamp}");
                    Log.Error(ex.Message);
                }
            }

        }
    }
}