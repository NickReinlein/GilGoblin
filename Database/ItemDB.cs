using GilGoblin.Finance;
using GilGoblin.WebAPI;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Timers;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace GilGoblin.Database
{
    internal class ItemDB
    {
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.None)]
        [Key]
        public int itemID { get; set; }

        public ItemInfoDB itemInfo { get; set; }
        public List<MarketDataDB> marketData { get; set; } = new List<MarketDataDB>();
        public List<RecipeDB> fullRecipes { get; set; } = new List<RecipeDB>();

        public ItemDB() { }

        public ItemDB(int itemID, int worldID)
        {
            this.itemID = itemID;
            MarketDataDB marketData = MarketDataDB.GetMarketDataSingle(itemID, worldID);
            this.marketData.Add(marketData);
            this.itemInfo = ItemInfoDB.GetItemInfo(itemID);

            if (this.itemInfo != null && this.itemInfo.fullRecipes.Count > 0)
            {
                this.fullRecipes = this.itemInfo.fullRecipes.ToList();
            }
            else
            {
                this.fullRecipes.Clear();
            }
        }

        /// <summary>
        /// Used for bulk processing ease-of-use: can take a single marketData and extract recipes
        /// so less parameters are required
        /// </summary>
        /// <param name="itemID">Item ID</param>
        /// <param name="itemInfo">General item information</param>
        /// <param name="marketData">Market data from one world</param>
        protected ItemDB(int itemID, ItemInfoDB itemInfo, MarketDataDB marketData)
        {
            this.itemID = itemID;
            this.itemInfo = itemInfo;

            List<MarketDataDB> listConvert = new List<MarketDataDB>();
            listConvert.Add(marketData);
            this.marketData = listConvert;

            if (this.itemInfo != null && this.itemInfo.fullRecipes.Count > 0)
            {
                this.fullRecipes = this.itemInfo.fullRecipes.ToList();
            }
            else
            {
                this.fullRecipes.Clear();
            }
        }

        /// <summary>
        /// Used internally for bulk processing when items are fetched in bulk
        /// </summary>
        /// <param name="itemID">Item ID</param>
        /// <param name="itemInfo">Basic general info</param>
        /// <param name="marketData">Marketdata list (per world)</param>
        /// <param name="fullRecipes">List of complete Recipes</param>

        protected ItemDB(int itemID, ItemInfoDB itemInfo, List<MarketDataDB> marketData, List<RecipeDB> fullRecipes)
        {
            this.itemID = itemID;
            // XIVAPI does not have a bulk API call AFAIK :(
            this.itemInfo = itemInfo;
            this.marketData = marketData;
            this.fullRecipes = fullRecipes;
        }

        public static List<ItemDB> bulkCreateItemDBFromLargeList(List<int> itemIDs)
        {
            List<ItemDB> listItemDB = new List<ItemDB>();
            int jumpCount = DatabaseAccess._entriesPerAPIPull;
            
            // Process a large list in batches to prevent API abuse
            for (int i = 0; i < itemIDs.Count; i = i+jumpCount){
                try
                {
                    List<int> subIDList = itemIDs.GetRange(i, jumpCount);
                    Stopwatch stopwatch = new Stopwatch();
                    Log.Debug("Starting creation of " + jumpCount + " items.");
                    List<ItemDB> subList = bulkCreateItemDB(subIDList);
                    stopwatch.Stop();
                    int seconds = (int) stopwatch.Elapsed.TotalSeconds;
                    Log.Debug("Done in " + seconds + " seconds.");
                    listItemDB.AddRange(subList);
                    float donePercent = ((float)i / (float)itemIDs.Count) * 100;
                    String percentString = donePercent.ToString("0.00");
                    Log.Information(percentString+"% done. Found:" +i+"/"+itemIDs.Count);
                    //Wait here
                    //Thread.Sleep(1000);
                }
                catch (Exception ex)
                {
                    Log.Error("Failed to fetch a sublist of items from a large list. Err:{errmessage}", ex.Message);
                }
            }
            return listItemDB;
        }

        public static List<ItemDB> bulkCreateItemDB(List<int> itemIDs)
        {
            return bulkCreateItemDB(itemIDs, Cost._default_world_id);
        }

        public static List<ItemDB> bulkCreateItemDB(List<int> itemIDs, int worldID)
        {
            List<ItemDB> listItemDB = new List<ItemDB>();
            List<MarketDataDB> marketData = new List<MarketDataDB>();

            try
            {
                marketData = MarketDataDB.GetMarketDataBulk(itemIDs, worldID,true);
                //marketData = MarketDataDB.ConvertWebToDBBulk(
                //    MarketDataWeb.FetchMarketDataBulk(itemIDs, worldID).GetAwaiter().GetResult());
            }
            catch (Exception ex)
            {
                Log.Error("Failed bulk fetch of market data with error message:{error}", ex.Message);
            }


            foreach (int i in itemIDs)
            {
                try
                {
                    ItemInfoDB itemInfo = ItemInfoDB.GetItemInfo(i);
                    if (itemInfo == null)
                    {
                        Log.Debug("Failed to get itemInfo for item with ID:{id}, skipping this.", i);
                        continue;
                    }

                    var marketDataDb = marketData.Find(t => t.itemID == i && t.worldID == worldID);
                    if (marketDataDb == null){
                        Log.Verbose("Did not find marketData for item with ID:{id}, worldID:{worldID}", i, worldID);
                    }

                    ItemDB newItem = new ItemDB(i, itemInfo, marketDataDb);
                    if (newItem != null && newItem.itemInfo.name.Length > 0)
                    {
                        listItemDB.Add(newItem);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Failed (bulk) creation of item ID:{itemID} with error message:{error}", i, ex.Message);
                }
            }

            Log.Debug("Bulk create: Created {create} of {req} entries requested in bulkCreateItemDB().", listItemDB.Count, itemIDs.Count);

            return listItemDB;

        }

        /// <summary>
        /// Pulls item using the default world ID
        /// </summary>
        /// <param name="itemIDList"></param> A list of integers represnting item ID
        /// <returns></returns>
        public static List<ItemDB> GetItemDBBulk(List<int> itemIDList)
        {
            return GetItemDBBulk(itemIDList, Cost._default_world_id);
        }

        /// <summary>
        /// Searches the database for a bulk list of items given their ID & world
        /// </summary>
        /// <param name="itemIDList"></param>A List of integers to represent item ID
        /// <param name="worldId"></param>the world ID for world-specific data (ie: market price)
        /// <returns></returns>
        public static List<ItemDB> GetItemDBBulk(List<int> itemIDList, int worldId)
        {
            if (worldId == 0 || itemIDList == null || itemIDList.Count == 0)
            {
                Log.Error("Trying to get item data with missing parameters.");
                return null;
            }
            try
            {
                List<ItemDB> returnList = new List<ItemDB>();

                List<ItemDB> exists;
                try
                {
                    using (ItemDBContext context = DatabaseAccess.getContext())
                    {
                        exists = context.data
                            .Where(t => itemIDList.Contains(t.itemID))
                            .Include(t => t.marketData)
                            .Include(t => t.fullRecipes)
                            .Include(t => t.itemInfo)
                            .ToList();
                    }
                }
                catch (System.NullReferenceException)
                {
                    exists = null;
                }
                if (exists != null &&
                    exists.Count > 0)
                {
                    if (exists.Count == itemIDList.Count)
                    {
                        //Everything has been found, return results
                        returnList = exists;
                    }
                    else
                    {
                        returnList.AddRange(exists);
                        IEnumerable<ItemDB> itemFetch = returnList.Except(exists);
                    }
                }
                else //Does not exist, so we have to fetch it
                {

                    List<ItemDB> fetchList = new List<ItemDB>();
                    fetchList = FetchItemDBBulk(itemIDList, worldId);
                }

                return returnList;
            }
            catch (Exception ex)
            {
                if (ex is SqliteException || ex is InvalidOperationException)
                {
                    //Maybe the database doesn't exist yet or not found
                    //Either way, we can return null -> it is not on the database
                    Log.Debug("Database or entry does not exist.");
                    return null;
                }
                else
                {

                    Log.Error("Exception: {message}.", ex.Message);
                    Log.Error("Inner exception:{inner}.", ex.InnerException);
                    DatabaseAccess.Disconnect();
                    return null; ;
                }
            }
        }

        /// <summary>
        /// Searches the database for MarketData (average market price, vendor price, etc)
        /// </summary>
        /// <param name="itemID"></param>item ID (ie: 5057 for Copper Ingot)
        /// <param name="worldID"></param>world ID (ie: 34 for Brynhildr)
        /// <returns></returns>
        public static ItemDB GetItemDBSingle(int itemID, int worldID)
        {
            List<int> itemIDList = new List<int>();
            itemIDList.Add(itemID);
            ItemDB returnItem = null;

            try
            {
                List<ItemDB> list = ItemDB.GetItemDBBulk(itemIDList, worldID);
                if (list.Count > 0)
                {
                    returnItem = list.First();
                }
                return returnItem;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                return null;
            }
        }

        public static List<ItemDB> FetchItemDBBulk(List<int> itemIDList, int worldId)
        {
            List<ItemDB> returnList = new List<ItemDB>();

            try
            {
                returnList = ItemDB.bulkCreateItemDB(itemIDList, worldId);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to get itemDB in item list in world {worldID} with message: {message}", worldId, ex.Message);
                return null;
            }
            return returnList;
        }
        public static ItemDB FetchItemDBSingle(int itemID, int worldId)
        {
            try
            {
                ItemDB itemDBFetched = new ItemDB(itemID, worldId);

                if (itemDBFetched == null)
                {
                    throw new Exception("Nothing returned from the FetchItemDBBulk() method.");
                }
                return itemDBFetched;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to get itemDB in FetchItemDBSingle() for item {itemID} world {worldID} with message: {message}", itemID, worldId, ex.Message);
                return null;
            }
        }        

    }
}
