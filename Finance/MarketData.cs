using GilGoblin.Database;
using GilGoblin.Finance;
using GilGoblin.WebAPI;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GilGoblin.Finance
{
    /// <summary>
    /// Manages market information such as market price 
    /// </summary>
    internal class MarketData
    {
        public int item_id { get; set; }
        public int world_id { get; set; }
        public DateTime last_updated { get; set; }
        public int average_Price { get; set; }        

        //Try with the database first, then if it fails we use the web API
        public static MarketDataDB GetMarketData(int item_id, int world_id)
        {
            MarketDataDB marketDataDB;
            //Does it exist in the database? Is it stale?
            try
            {
                marketDataDB = Database.DatabaseAccess.GetMarketDataDB(item_id, world_id);
            }
            catch(Exception)
            {                
                //Not found in the database)
                marketDataDB = null;
            }

            //If found, stop & return
            if (marketDataDB != null){ return marketDataDB; }
            else 
            { 
                //fetch with the web api
                MarketDataWeb marketDataWeb
                    = MarketDataWeb.FetchMarketData(item_id, world_id).GetAwaiter().GetResult();
                marketDataDB = new MarketDataDB(marketDataWeb);
                return marketDataDB;
            }
        }

        public static ItemInfo GetItemInfo(int item_id)
        {
            //TODO: later this needs to be doneDatabase.DatabaseAccess.GetMarketDataDB
            // and probably move to a new namespace or class
            return MarketDataWeb.FetchItemInfo(item_id).GetAwaiter().GetResult();

        }

        /// <summary>
        /// Given the collection of listings, calculate their average price
        /// </summary>
        /// <param name="listings"></param>
        /// <returns></returns>
        public int CalculateAveragePrice(ICollection<MarketListing> listings)
        {
            int average_Price = 0;
            uint total_Qty = 0;
            ulong total_Gil = 0;
            foreach (MarketListingWeb listing in listings)
            {
                total_Qty += ((uint)listing.qty);
                total_Gil += ((ulong)(listing.price * listing.qty));
            }
            if (total_Qty > 0)
            {
                float average_Price_f = total_Gil / total_Qty;
                average_Price = (int)Math.Round(average_Price_f);
            }
            return average_Price;
        }

        internal int CalculateAveragePrice(ICollection<MarketListingWeb> listings)
        {
            int average_Price = 0;
            uint total_Qty = 0;
            ulong total_Gil = 0;
            foreach (MarketListingWeb listing in listings)
            {
                total_Qty += ((uint)listing.qty);
                total_Gil += ((ulong)(listing.price * listing.qty));
            }
            if (total_Qty > 0)
            {
                float average_Price_f = total_Gil / total_Qty;
                average_Price = (int)Math.Round(average_Price_f);
            }
            return average_Price;
        }
    }
}


