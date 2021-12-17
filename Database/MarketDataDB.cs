using System;
using System.Collections.Generic;
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
        public ICollection<MarketListingDB> listings { get; set; } 
            = new List<MarketListingDB>();
        public MarketDataDB() : base() { }
        public MarketDataDB(MarketDataWeb data) : base() 
        {
            //Convert each market listing from the web format to DB
            foreach (MarketListingWeb web in data.listings)
            {
                listings.Add(web.ConvertToDB());
            }
            this.last_updated = data.last_updated;
            this.average_Price = data.average_Price;
            this.item_id = data.item_id;
            this.world_id = data.world_id;

        }
    }
}
