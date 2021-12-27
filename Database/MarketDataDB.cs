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
        public int averagePrice { get; set; }

        public MarketDataDB() : base() { }
        public MarketDataDB(MarketDataWeb data) : base()
        {
            if (data.listings != null && data.listings.Count > 0)
            {
                //Convert each market listing from the web format to DB
                foreach (MarketListingWeb web in data.listings)
                {
                    listings.Add(web.ConvertToDB());
                }
            }
            this.lastUpdated = DateTime.Now;
            this.averagePrice = (int) data.averagePrice;
            this.itemID = data.itemID;
            this.worldID = data.worldID;
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
                    this.itemID = marketDataWeb.itemID;
                    this.worldID = marketDataWeb.worldID;
                    this.averagePrice = (int) marketDataWeb.averagePrice;
                    this.lastUpdated = marketDataWeb.lastUpdated;

                    //Convert each market listing from the web format to DB
                    foreach (MarketListingWeb web in marketDataWeb.listings)
                    {
                        listings.Add(web.ConvertToDB());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Failed to fetch market data for itemID {itemID}, worldID {worldID} with message {message}.", base.itemID, base.worldID, ex.Message);
            }
        }

        internal static MarketDataDB GetMarketDataSingle(int itemID, int worldID
            , bool forceUpdate = false)
        {
            List<int> itemIDList = new List<int>();
            List<MarketDataDB> returnList = new List<MarketDataDB>();
            MarketDataDB returnMe = null;

            itemIDList.Add(itemID);
            try
            {
                returnList = GetMarketDataBulk(itemIDList, worldID, forceUpdate);
                returnMe = returnList[0];
            }
            catch (Exception ex)
            {
                Log.Error("Failed to fetch market data for itemID {itemID}, worldID {worldID} with message {message}.", itemID, worldID, ex.Message);
                returnMe = null;
            }
            return returnMe;
        }

            /// <summary>
            /// Given a List<int> that represents the item_id, along with the world_id,
            /// get the market data for each item and return it as a List
            /// </summary>
            /// <param name="dict"></param>A Dictionary<int,int> that represents the item_id and world_id
            /// <returns></returns>
            internal static List<MarketDataDB> GetMarketDataBulk(List<int> itemIDs, int world_ID, bool forceUpdate = false)
        {
            if (world_ID == 0 || itemIDs == null || 
                itemIDs.Count == 0)
            {
                Log.Error("Trying to get bulk market data with missing parameters.");
                return null;
            }
            else
            {
                List<MarketDataDB> listReturn = new List<MarketDataDB>();
                List<ItemDB> listDB = new List<ItemDB>();
                //Does it exist in the database? Is it stale?
                try
                {
                    if (!forceUpdate)
                    {
                        listDB = ItemDB.GetItemDBBulk(itemIDs, world_ID);
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
                    itemIDFetchList.Remove(success.itemID);
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
                DatabaseAccess.context.AddRange(listReturn);
                return listReturn;
            }
        }

        public static List<MarketDataDB> filterFreshData(List<ItemDB> list)
        {
            List<MarketDataDB> fresh = new List<MarketDataDB>();
            if (list == null) { return fresh; }

            foreach (ItemDB itemDB in list)
            {
                if (itemDB == null) { continue; }
                foreach (MarketDataDB marketData in itemDB.marketData)
                {
                    if (!isDataStale(marketData)) { fresh.Add(marketData); }
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
            TimeSpan diff = DateTime.Now - marketData.lastUpdated;
            double hoursElapsed = diff.TotalHours;
            if (hoursElapsed > MarketData._staleness_hours_for_refresh) { return true; }
            else { return false; }
        }
    }
}
