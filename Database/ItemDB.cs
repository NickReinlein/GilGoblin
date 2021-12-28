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

            DatabaseAccess.context.Add(this);
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
                ItemDBContext context = DatabaseAccess.context;
                List<ItemDB> returnList = new List<ItemDB>();

                List<ItemDB> exists = context.data
                        .Where(t => itemIDList.Contains(t.itemID))
                        .Include(t => t.marketData)
                        .Include(t => t.fullRecipes)
                        .Include(t => t.itemInfo)                     
                        .ToList();
                if (exists != null &&
                    exists.Count > 0)
                {
                    if (exists.Count == itemIDList.Count)
                    {
                        //Everything has been found, return results
                        context.UpdateRange(exists);
                        returnList = exists;
                    }
                    else
                    {
                        returnList.AddRange(exists);
                        IEnumerable<ItemDB> itemFetch = returnList.Except(exists);

                        //Non-existent entries are added to the tracker
                        foreach (ItemDB newItem in returnList)
                        {
                            if (newItem != null)
                            {
                                returnList.Add(newItem);
                                context.AddAsync<ItemDB>(newItem);
                            }
                        }                        
                    }
                    context.SaveChangesAsync();
                }
                //else //Does not exist, so we have to fetch it
                //{

                //    List<ItemDB> fetchList = new List<ItemDB>();
                //    fetchList = FetchItemDBBulk(itemIDList, worldId);
                //    if (fetchList.Count > 0)
                //    {
                //        context.AddRange(fetchList);
                //    }
                //}

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

            foreach (int itemID in itemIDList)
            {
                ItemDB itemDB = new ItemDB(itemID, worldId);
                if (itemDB != null) { returnList.Add(itemDB); }
            }

            return returnList;
        }
        public static ItemDB FetchItemDBSingle(int itemID, int worldId)
        {
            try
            {
                ItemDB fetchMe = null;
                List<int> IDAsList = new List<int>();
                IDAsList.Add(itemID);

                List<ItemDB> itemList = FetchItemDBBulk(IDAsList, worldId);
                if (itemList.Count > 0) {
                    fetchMe = itemList[0];
                }

                if (fetchMe == null)
                {
                    throw new Exception("Nothing returned from the FetchItemDBBulk() method.");
                }
                ItemDBContext context = DatabaseAccess.context;
                context.Add(fetchMe);
                return fetchMe;
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
