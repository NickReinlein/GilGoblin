using GilGoblin.Finance;
using GilGoblin.WebAPI;
using Serilog;
using System;
using System.Collections.Generic;

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
            this.last_updated = DateTime.Now;
            this.average_price = data.average_price;
            this.item_id = data.item_id;
            this.world_id = data.world_id;
        }

        public MarketDataDB(int itemID, int worldID) : base()
        {
            try
            {
                MarketDataWeb marketDataWeb
                    = MarketDataWeb.FetchMarketData(itemID, worldID).GetAwaiter().GetResult();
                if (marketDataWeb == null)
                {
                    throw new Functions.DBStatusException();
                }
                else 
                {
                    this.item_id = marketDataWeb.item_id;
                    this.world_id = marketDataWeb.world_id;
                    this.average_price = marketDataWeb.average_price;
                    this.last_updated = marketDataWeb.last_updated;

                    //Convert each market listing from the web format to DB
                    foreach (MarketListingWeb web in marketDataWeb.listings)
                    {
                        listings.Add(web.ConvertToDB());
                    }
                }
            }
            catch (Exception)
            {
                Log.Error("Failed to fetch market data for itemID {itemID}, worldID {worldID}.", item_id, world_id);
            }
        }
    }
}
