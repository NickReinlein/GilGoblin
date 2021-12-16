using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GilGoblin.Finance
{
    internal class MarketListing
    {
        public bool hq { get; set; }
        public int price { get; set; }
        public int qty { get; set; }

        public MarketListing() { }

        public MarketListing(MarketListing other) 
        { 
            hq = other.hq;
            price = other.price;
            qty = other.qty;
        }
    }
}
