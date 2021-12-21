using GilGoblin.Database;
using GilGoblin.WebAPI;
using Serilog;
using System;
using System.Collections.Generic;

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
        public int average_price { get; set; }

        public static int _staleness_hours_for_refresh = 2; //todo: increase for production

        //Try with the database first, then if it fails we use the web API
        public static MarketDataDB GetMarketData(int item_id, int world_id)
        {
            MarketDataDB marketDataDB;
            //Does it exist in the database? Is it stale?
            try
            {
                marketDataDB = Database.DatabaseAccess.GetMarketDataDB(item_id, world_id);
            }
            catch (Exception)
            {
                //Not found in the database)
                marketDataDB = null;
            }

            //If found, stop & return
            if (marketDataDB != null) { return marketDataDB; }
            else
            {
                //fetch with the web api
                MarketDataWeb marketDataWeb
                    = MarketDataWeb.FetchMarketData(item_id, world_id).GetAwaiter().GetResult();
                marketDataDB = new MarketDataDB(marketDataWeb);
                return marketDataDB;
            }
        }

        /// <summary>
        /// Given a List<int> that represents the item_id, along with the world_id,
        /// get the market data for each item and return it as a List
        /// </summary>
        /// <param name="dict"></param>A Dictionary<int,int> that represents the item_id and world_id
        /// <returns></returns>
        internal static List<MarketDataDB> GetMarketDataBulk(List<int> itemIDs, int world_ID, bool forceUpdate = false)
        {
            if (world_ID == 0 || itemIDs == null || itemIDs.Count == 0)
            {
                Log.Error("Trying to get bulk market data with missing parameters.");
                return null;
            }
            else
            {
                List<MarketDataDB> listReturn = new List<MarketDataDB>();
                List<MarketDataDB> listDB = new List<MarketDataDB>();
                //Does it exist in the database? Is it stale?
                try
                {
                    if (!forceUpdate)
                    {
                        listDB = DatabaseAccess.GetMarketDataDBBulk(itemIDs, world_ID);
                    }
                }
                catch (Exception ex)
                {
                    // Error and/or not found in the database
                    Log.Information(ex.Message);
                }

                //Trim the list of entries back from the database
                //Return the ones that need to be fetched
                //This should included stale data & ones new to the database
                List<MarketDataDB> freshDataDB = new List<MarketDataDB>();
                freshDataDB = filterFreshData(listDB);
                List<int> itemIDFetchList = itemIDs;
                foreach (MarketDataDB success in freshDataDB)
                {
                    listReturn.Add(success);
                    itemIDFetchList.Remove(success.item_id);
                }

                Log.Debug("Number of entries to fetch by web: " + itemIDFetchList.Count);
                List<MarketDataWeb> listWeb = new List<MarketDataWeb>();
                if (itemIDFetchList.Count == 1)
                {
                    int fetchSingle = (int)itemIDFetchList[0];
                    listWeb.Add(MarketDataWeb.FetchMarketData(fetchSingle, world_ID)
                        .GetAwaiter().GetResult());
                }
                if (itemIDFetchList.Count > 1)
                {
                    listWeb = MarketDataWeb.FetchMarketDataBulk(itemIDFetchList, world_ID).GetAwaiter().GetResult();
                }
                if (listWeb != null)
                {
                    foreach (MarketDataWeb web in listWeb)
                    {
                        listReturn.Add(new MarketDataDB(web));
                    }
                }
                return listReturn;
            }
        }

        public static List<MarketDataDB> filterFreshData(List<MarketDataDB> list)
        {
            List<MarketDataDB> fresh = new List<MarketDataDB>();
            if (list != null)
            {
                foreach (MarketDataDB dataDB in list)
                {
                    if (!isDataStale(dataDB))
                    {
                        fresh.Add(dataDB);
                    }
                    else { } //do nothing                       
                }
            }
            return fresh;

        }

        /// <summary>
        /// Verifies if the data is too stale/old
        /// </summary>
        /// <param name="marketData"></param>The object of market data in DB format
        /// <returns></returns>
        public static bool isDataStale(MarketDataDB marketData)
        {
            //Check for freshness: if old, update data
            TimeSpan diff = DateTime.Now - marketData.last_updated;
            double hoursElapsed = diff.TotalHours;
            if (hoursElapsed > MarketData._staleness_hours_for_refresh) { return true; }
            else { return false; }
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

    internal class MarketDataBulk
    {
        ICollection<MarketData> bulkData = new List<MarketData>();

        MarketDataBulk()
        {
        }
    }

}


