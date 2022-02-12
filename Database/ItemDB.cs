using GilGoblin.Finance;
using GilGoblin.WebAPI;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

        public static List<ItemDB> bulkCreateItemDB(List<int> itemIDs)
        {
            return bulkCreateItemDB(itemIDs, Cost._default_world_id);
        }

            public static List<ItemDB> bulkCreateItemDB(List<int> itemIDs, int worldID)
        {
            List<ItemDB> listItemDB = new List<ItemDB>();
            try
            {
                List<MarketDataDB> marketData = MarketDataDB.ConvertWebToDBBulk(
                    MarketDataWeb.FetchMarketDataBulk(itemIDs, worldID).GetAwaiter().GetResult());


                foreach (int i in itemIDs)
                {
                    ItemInfoDB itemInfo = ItemInfoDB.FetchItemInfo(i);
                    MarketDataDB marketDataDb = marketData.Find(t => t.itemID == i);
                    if (marketDataDb == null || itemInfo == null) { continue; }
                    ItemDB newItem = new ItemDB(i, itemInfo, marketDataDb);
                    if (newItem != null) { listItemDB.Add(newItem); }
                }

                //DatabaseAccess.context.AddRange(listItemDB);

                Log.Debug("Bulk create: Created {create} entries out of {req} requested.", listItemDB.Count, itemIDs.Count);
            }
            catch (Exception ex)
            {
                Log.Error("Failed bulk created with error message:{ex.m}",ex.Message);
            }
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

                        ////Non-existent entries are added to the tracker
                        //foreach (ItemDB newItem in returnList)
                        //{
                        //    if (newItem != null)
                        //    {
                        //        returnList.Add(newItem);
                        //        context.AddAsync<ItemDB>(newItem);
                        //    }
                        //}                        
                    }
                    //context.SaveChangesAsync();
                }
                else //Does not exist, so we have to fetch it
                {

                    List<ItemDB> fetchList = new List<ItemDB>();
                    fetchList = FetchItemDBBulk(itemIDList, worldId);
                    //if (fetchList.Count > 0)
                    //{
                    //    context.AddRange(fetchList);
                    //}
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

        // Try with the database first, then if it fails we use the web API
        public static MarketDataDB GetMarketData(int itemID, int worldID)
        {
            ItemDB itemDB;
            MarketDataDB returnData;
            //Does it exist in the database? Is it stale?
            try
            {
                itemDB = GetItemDBSingle(itemID, worldID);
                if (itemDB != null)
                {
                    //Found, stop & return
                    returnData = itemDB.marketData.First(t => t.worldID == worldID);
                    return returnData;
                }
            }
            catch (Exception)
            {
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
            catch(Exception ex)
            {
                Log.Error("failed to fetch the market data via API for item {itemID}, world {worldID} with message {mesage}", itemID, worldID, ex.Message);
                return null;
            }
        }
    }
}
