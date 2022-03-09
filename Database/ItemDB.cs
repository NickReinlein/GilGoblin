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

namespace GilGoblin.Database {
    internal class ItemDB {
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
        /// <param name="marketData">Optional: Market data from one world</param>
        protected ItemDB(int itemID, ItemInfoDB itemInfo, MarketDataDB marketData = null)
        {
            this.itemID = itemID;
            this.itemInfo = itemInfo;

            if (marketData != null)
            {
                List<MarketDataDB> listConvert = new List<MarketDataDB>();
                listConvert.Add(marketData);
                this.marketData = listConvert;
            }

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

        /// <summary>
        /// Creates new items. Looks for the basic information (name, description, etc) but does not fetch market data such as market listings (and therefore average price)
        /// </summary>
        /// <param name="itemIDs">A list of item IDs to create</param>
        /// <returns></returns>
        public static List<ItemDB> bulkCreateItemBasicInfo(List<int> itemIDs)
        {
            List<ItemDB> listItemDB = new List<ItemDB>();
            int jumpCount = DatabaseAccess._entriesPerAPIPull;

            // Process a large list in batches to prevent API abuse
            for (int i = 0; i < itemIDs.Count; i = i + jumpCount)
            {
                try
                {
                    List<int> subIDList = itemIDs.GetRange(i, jumpCount);
                    Stopwatch stopwatch = new Stopwatch();
                    Log.Debug("Starting creation of " + jumpCount + " items.");
                    List<ItemDB> subList = bulkCreateItemDB(subIDList, true, false);
                    stopwatch.Stop();
                    int milliseconds = (int)stopwatch.Elapsed.TotalMilliseconds;
                    Log.Debug("Done in " + milliseconds + " milliseconds.");
                    listItemDB.AddRange(new List<ItemDB>(subList));
                    float donePercent = ((float)i / (float)itemIDs.Count) * 100;
                    String percentString = donePercent.ToString("0.00");
                    Log.Information(percentString + "% done. Found:" + i + "/" + itemIDs.Count);
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

        // Default world ID is used
        public static List<ItemDB> bulkCreateItemDB(List<int> itemIDs, bool skipMarketData = false, bool skipDBCheck = false)
        {
            return bulkCreateItemDB(itemIDs, Cost._default_world_id, skipMarketData, skipDBCheck);
        }

        // World ID is provided for market data
        public static List<ItemDB> bulkCreateItemDB(List<int> itemIDs, int worldID, bool skipMarketData = false, bool skipDBCheck = false)
        {
            List<ItemDB> listItemDB = new List<ItemDB>();
            List<MarketDataDB> marketData = new List<MarketDataDB>();

            if (!skipMarketData)
            {
                try
                {
                    marketData = MarketDataDB.GetMarketDataBulk(itemIDs, worldID, true);
                }
                catch (Exception ex)
                {
                    Log.Error("Failed bulk fetch of market data with error message:{error}", ex.Message);
                }
            }

            int count = 0;
            foreach (int i in itemIDs)
            {
                try
                {
                    count++;
                    ItemInfoDB itemInfo = ItemInfoDB.GetItemInfo(i);
                    if (itemInfo == null)
                    {
                        Log.Debug("Failed to get itemInfo for item with ID:{id}, skipping this.", i);
                        continue;
                    }
                    ItemDB newItem;

                    if (!skipMarketData)
                    {
                        var marketDataDb = marketData.Find(t => t.itemID == i && t.worldID == worldID);
                        if (marketDataDb == null)
                        {
                            Log.Verbose("Did not find marketData for item with ID:{id}, worldID:{worldID}", i, worldID);
                        }
                        newItem = new ItemDB(i, itemInfo, marketDataDb);
                    }
                    else
                    {
                        newItem = new ItemDB(i, itemInfo);
                    }

                    if (newItem != null && newItem.itemInfo.name.Length > 0)
                    {
                        listItemDB.Add(newItem);
                    }

                    // Print progress %
                    if (count % DatabaseAccess._entriesPerAPIPull == 0){
                        float donePercent = ((float)count / (float)itemIDs.Count) * 100;
                        String percentString = donePercent.ToString("0.00");
                        Log.Information(percentString + "% done. Found:" + count + "/" + itemIDs.Count);
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
        /// Searches the database for a bulk list of items given their ID & world. If they are not found in the database, they are fetched via GET web request, then saved.
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
                List<ItemDB> newlyCreatedItems = new List<ItemDB>();
                List<int> remainingItemIDList = new List<int>(itemIDList);

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
                catch (Exception)
                {
                    exists = null;
                }


                if (exists != null && exists.Count > 0){
                    // Add what we found to our return variable, fetch the rest
                    Log.Debug("Found {entries} entries in the database of the {total} requested.", exists.Count, itemIDList.Count);
                    returnList.AddRange(exists);
                    foreach (ItemDB itemDB in exists){
                        remainingItemIDList.Remove(itemDB.itemID); 
                    }                    
                }

                if (remainingItemIDList.Count > 0)
                {
                    // Create many items, skip checking the DB, skip getting market data for initial startup
                    Log.Debug("{entries} entries not found in the database of the {total} requested. Fetching these with GET request.", remainingItemIDList.Count, itemIDList.Count);
                    newlyCreatedItems = bulkCreateItemDB(remainingItemIDList, worldId, true, true);

                    returnList.AddRange(newlyCreatedItems);

                    using (ItemDBContext context = DatabaseAccess.getContext()){
                        context.AddRange(newlyCreatedItems);
                        DatabaseAccess.Save(context);
                    }
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

        public static List<ItemDB> FetchItemDBBulk(List<int> itemIDList, int worldId, bool skipMarketData = false, bool skipDBCheck = false){
            List<ItemDB> returnList = new List<ItemDB>();

            try
            {
                returnList = ItemDB.bulkCreateItemDB(itemIDList, worldId, skipMarketData,skipDBCheck);
            }
            catch (Exception ex)
            {
                Log.Error("Failed to get itemDB in item list in world {worldID} with message: {message}", worldId, ex.Message);
                return null;
            }
            return returnList;
        }
        public static ItemDB FetchItemDBSingle(int itemID, int worldID, bool skipMarketData = false, bool skipDBCheck = false)
            {
            try
            {
                List<int> itemIDList = new List<int>();
                itemIDList.Add(itemID);
                ItemDB itemDBFetched = FetchItemDBBulk(itemIDList, worldID, skipMarketData, skipDBCheck).First();

                if (itemDBFetched == null)
                {
                    throw new Exception("Nothing returned from the FetchItemDBBulk() method.");
                }
                return itemDBFetched;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to get itemDB in FetchItemDBSingle() for item {itemID} world {worldID} with message: {message}", itemID, worldID, ex.Message);
                return null;
            }
        }

    }
}
