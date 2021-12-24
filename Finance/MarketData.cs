using GilGoblin.Database;
using GilGoblin.WebAPI;
using Newtonsoft.Json;
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
            ItemDB itemDB;
            MarketDataDB returnData;
            //Does it exist in the database? Is it stale?
            try
            {
                itemDB = ItemDB.GetItemDataDB(item_id, world_id);
            }
            catch (Exception)
            {
                //Not found in the database)
                itemDB = null;
            }

            //If found, stop & return
            if (itemDB != null) { returnData = itemDB.marketData; }
            else
            {
                //fetch with the web api
                MarketDataWeb marketDataWeb
                    = MarketDataWeb.FetchMarketData(item_id, world_id).GetAwaiter().GetResult();
                MarketDataDB newData = new MarketDataDB(marketDataWeb);
                returnData = newData;
            }
            return returnData;
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

        public static List<MarketDataDB> filterFreshData(List<ItemDB> list)
        {
            List<MarketDataDB> fresh = new List<MarketDataDB>();
            if (list != null)
            {
                foreach (ItemDB itemDB in list)
                {
                    if (itemDB != null && 
                        !isDataStale(itemDB.marketData))
                    {
                        fresh.Add(itemDB.marketData);
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

        /// <summary>
        /// Given the collection of listings, calculate their average price
        /// </summary>
        /// <param name="listings"></param>
        /// <returns></returns>
        public int CalculateAveragePrice(ICollection<MarketListingWeb> listings)
        {
            int average_Price = 0;
            uint total_Qty = 0;
            ulong total_Gil = 0;
            foreach (MarketListingWeb listing in listings)
            {                
                if ((listing.qty > 1 && listing.price > 200000) ||
                    (listing.qty == 1 && listing.price > 400000))
                {
                    continue; //Exclude crazy prices
                }
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


