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
        public DateTime timestamp { get; set; }
    }
}
