using GilGoblin.Finance;
using GilGoblin.WebAPI;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;

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
            this.averagePrice = (int)data.averagePrice;
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
                    this.averagePrice = (int)marketDataWeb.averagePrice;
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

        public static List<MarketDataDB> ConvertWebToDBBulk(List<MarketDataWeb> list)
        {
            List<MarketDataDB> returnList = new List<MarketDataDB>();

            try
            {
                foreach (MarketDataWeb web in list)
                {
                    MarketDataDB db = new MarketDataDB(web);
                    returnList.Add(db);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Failed to convert MarketDataWeb to DB in bulk. Message:{ex.message}", ex.Message);
            }

            return returnList;
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
                try
                {
                    List<MarketDataDB> freshDataDB = new List<MarketDataDB>();
                    freshDataDB = filterFreshData(listDB);
                    List<int> itemIDFetchList = new List<int>(itemIDs);
                    foreach (MarketDataDB success in freshDataDB)
                    {
                        listReturn.Add(success);
                        itemIDFetchList.Remove(success.itemID);
                    }

                    List<MarketDataWeb> listWeb = new List<MarketDataWeb>();
                    if (itemIDFetchList.Count == 1)
                    {
                        int fetchSingle = (int)itemIDFetchList[0];
                        listWeb.Add(MarketDataWeb.FetchMarketData(fetchSingle, world_ID)
                            .GetAwaiter().GetResult());
                    }
                    else if (itemIDFetchList.Count > 1)
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
                    //DatabaseAccess.context.AddRange(listReturn);
                    return listReturn;
                }
                catch (Exception ex)
                {
                    Log.Information(ex.Message);
                    return null;
                }

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

        // Try with the database first, then if it fails we use the web API
        public static MarketDataDB GetMarketData(int itemID, int worldID)
        {
            ItemDB itemDB;
            MarketDataDB returnData;
            //Does it exist in the database? Is it stale?
            try
            {
                itemDB = ItemDB.GetItemDBSingle(itemID, worldID);
                if (itemDB != null){ //Found, stop & return
                    returnData = itemDB.marketData.First(t => t.itemID == itemID && t.worldID == worldID);
                    return returnData;
                }
            }
            catch (Exception){
                // Not found in the database
                itemDB = null;
            }

            try
            {
                // Not on database, fetch with the web api
                MarketDataWeb marketDataWeb
                    = MarketDataWeb.FetchMarketData(itemID, worldID).GetAwaiter().GetResult();
                MarketDataDB newData = new MarketDataDB(marketDataWeb);
                returnData = newData;
                return returnData;
            }
            catch (Exception ex)
            {
                Log.Error("failed to fetch the market data via API for item {itemID}, world {worldID} with message {mesage}", itemID, worldID, ex.Message);
                return null;
            }
        }
    }
}
