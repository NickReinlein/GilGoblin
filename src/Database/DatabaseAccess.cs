using GilGoblin.Crafting;
using GilGoblin.Pocos;
using GilGoblin.Web;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static GilGoblin.Pocos.RecipePoco;

namespace GilGoblin.Database
{
    internal class DatabaseAccess
    {
        public static string _file_path = Path.GetDirectoryName(AppContext.BaseDirectory);
        public static string _db_name = "GilGoblin.db";
        public static string _path = Path.Combine(_file_path, _db_name);
        public const int _initialDBCreationEntryCount = 8320;
        public const int _entriesPerAPIPull = 20;
        public const int _waitTimeInMsForAPICalls = 500;
        public const int _gameItemTotalCount = 36700;

        public static SqliteConnection _conn { get; set; }


        /// <summary>
        /// Gets a context for EF regarding the ItemID: use then discard!
        /// </summary>
        /// <returns></returns>
        public static GilGoblinDbContext getContext()
        {
            return new GilGoblinDbContext();
        }

        internal static SqliteConnection? Connect()
        {
            try
            {
                if (_conn == null)
                {
                    _conn = new SqliteConnection("Data Source=" + _path);
                }

                //Already open, return
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    return _conn;
                }

                _conn.Open();
                if (_conn.State == System.Data.ConnectionState.Open)
                {
                    return _conn;
                }
                else
                {
                    Log.Error("Connection not open. State is: {state}.", _conn.State);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Log.Error("failed connection:{message}.", ex.Message);
                return null;
            }
        }

        public static void Disconnect()
        {
            if (DatabaseAccess._conn != null &&
            DatabaseAccess._conn.State != System.Data.ConnectionState.Closed)
            {
                try
                {
                    _conn.Close();
                    _conn.Dispose();
                }
                catch (Exception ex)
                {
                    Log.Error(ex.Message);
                }
            }

        }

        public static void Startup()
        {
            try
            {
                Log.Information("Startup initiated.");

                bool initial = false;
                try
                {
                    using (GilGoblinDbContext context = getContext())
                    {

                        int itemCount = context.Data.Count();
                        if (itemCount < _initialDBCreationEntryCount)
                        {
                            initial = true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.Debug($"Exception during startup. Database may not be set up yet. Trying initial setup next. Message: {ex.Message}.", ex.Message);
                    initial = true;
                }

                if (initial) { InitialStartup(); }

            }
            catch (Exception ex)
            {
                Log.Debug("Database startup did not succed: {ex.Message}", ex.Message);
                return;
            }
        }

        public static void InitialStartup()
        {
            try
            {
                HashSet<ItemDB> initialItemRun = new HashSet<ItemDB>();
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                // TODO: Get the world ID fed here so we can pull for the right world ID)            
                //List<int> batchItemIDList = CraftingList.getListOfCraftableItemIDsByClass(CraftingClass.armorer);
                List<int> batchItemIDList = CraftingList.getListOfAllCraftableItemIDs();
                List<int> shortList = batchItemIDList.GetRange(0, Math.Min(_initialDBCreationEntryCount, batchItemIDList.Count));
                Log.Information("One-time inital database startup. Timer started. Starting with {itemCount} items.", shortList.Count);
                if (shortList.Count > 0)
                {
                    var thisBatchOfItems = ItemDB.GetItemDBBulk(shortList);
                    initialItemRun.UnionWith(new List<ItemDB>(thisBatchOfItems));
                    Log.Debug("Returned from bulk get with {numberOfrecords} records, for a total of {totalRecords}.", thisBatchOfItems.Count(), initialItemRun.Count);
                    stopwatch.Stop();
                    TimeSpan ts = stopwatch.Elapsed;

                    // Format and display the TimeSpan value.
                    string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);
                    Log.Information("Done initial startup. Total runTime is: {elapsedTime}, or {speed} items/second.", elapsedTime, thisBatchOfItems.Count / Math.Max(1, ts.Seconds));
                }
                else
                {
                    Log.Error("Failed to intialize database: initial list for item ID is empty!");
                    throw new("Failed create initial item ID list for database startup.");
                }

                if (initialItemRun.Count == 0)
                {
                    Log.Error("Tried to fetch {tryCount} items but found none! Nothing saved.", shortList.Count);
                    throw new Functions.OperationException("Failed create initial item ID list for database startup.");
                }
                else if (initialItemRun.Count != shortList.Count)
                {
                    Log.Warning("Tried to fetch {tryCount} items but found {foundCount}. Saving the items found successfully.");
                }
                else
                {
                    Log.Debug("Found all {tryCount} items. Saving.", shortList.Count);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Failed initial database startup with error: {message}", ex.Message);
            }

        }

        public static void Save(GilGoblinDbContext context)
        {
            try
            {
                Log.Debug("Saving to database.");
                context.Database.EnsureCreated();
                int savedEntries = context.SaveChanges();
                Log.Debug("Saved {saved} entries to the database.", savedEntries);
                context.Dispose();
            }
            catch (Exception ex)
            {
                Log.Debug("Database save did not succeed! Message: {message}. Inner Message:{inner}", ex.Message, ex.InnerException);
            }
        }

        /// <summary>
        /// Saves a list of marketDataDB items (with listings) 
        /// This can be done in bulk much more efficiently but requires refactoring & testing
        /// Will revisit if performance is an issue
        /// </summary>
        /// <param name="marketDataList"></param> a List<MarketData> that will be saved to the database
        /// <returns></returns>
        internal static async Task<int> SaveMarketDataBulk(List<MarketDataDB> marketDataList)
        {
            if (marketDataList == null || marketDataList.Count == 0)
            {
                Log.Error("Trying to save an empty list of market data.");
                return 0;
            }
            else
            {
                try
                {
                    using (GilGoblinDbContext context = getContext())
                    {
                        HashSet<int> itemIDList = new HashSet<int>();
                        foreach (MarketDataDB marketData in marketDataList)
                        {
                            if (marketData != null)
                            {
                                itemIDList.Add(marketData.itemID);
                            }
                        }

                        if (itemIDList.Count == 0)
                        {
                            Log.Error("Could not save market data due to missing item ID and world ID list.");
                            return 0;
                        }

                        List<ItemDB> itemDBExists = context.data
                                .Where(t => (marketDataList.All(x => itemIDList.Contains(x.itemID))))
                                .Include(t => t.marketData)
                                .Include(t => t.fullRecipes)
                                .Include(t => t.itemInfo)
                                .ToList();
                        List<MarketDataDB> exists = new List<MarketDataDB>();

                        if (itemDBExists == null)
                        {
                            //New entries, add to entity tracker     
                            await context.AddRangeAsync(marketDataList);
                        }
                        else
                        {

                            //Existing entries get updated
                            foreach (MarketDataDB exist in exists)
                            {
                                //Existing entry
                                exist.averagePrice = exist.averagePrice;
                                exist.listings = exist.listings;
                                context.Update<MarketDataDB>(exist);
                            }
                            //Non-existent entries are added to the tracker
                            foreach (MarketDataDB newData in marketDataList)
                            {
                                var thisExists = exists
                                    .Find(t => t.itemID == newData.itemID &&
                                               t.worldID == newData.worldID);
                                if (thisExists == null)
                                {
                                    await context.AddAsync<MarketDataDB>(newData);
                                }
                            }
                        }

                        return await context.SaveChangesAsync(); ;

                    }
                }
                catch (Exception ex)
                {
                    Log.Error("Exception: {message}.", ex.Message);
                    Log.Error("{NewLine}Inner exception: {innser}.", ex.InnerException);
                    Disconnect();
                    return 0;
                }
            }
        }

        internal static async Task<int> SaveMarketDataSingle(MarketDataDB marketData, bool saveToDB = true)
        {
            try
            {
                using (GilGoblinDbContext context = getContext())
                {
                    ItemDB itemExists = context.data
                    .FindAsync(marketData.itemID, marketData.worldID).GetAwaiter().GetResult();

                    if (itemExists == null)
                    {
                        //New entry, add to entity tracker
                        await context.AddAsync<MarketDataDB>(marketData);
                    }
                    else
                    {
                        MarketDataDB exists = itemExists.marketData
                            .Find(t => t.itemID == marketData.itemID &&
                                       t.worldID == marketData.worldID);
                        //Existing entry
                        if (exists == null)
                        {
                            Log.Error("No market data while saving for item {itemID} & world {worldID}.", itemExists.itemInfo.itemID, marketData.worldID);
                            return 0;
                        }
                        exists.lastUpdated = DateTime.Now;
                        exists.averagePrice = marketData.averagePrice;
                        exists.listings = marketData.listings;
                        context.Update<ItemDB>(itemExists);
                    }
                    int success = 0;
                    if (saveToDB) { success = await context.SaveChangesAsync(); }
                    return success;
                }
            }
            catch (Exception ex)
            {
                Log.Error("Exception: {message}.", ex.Message);
                Log.Error("Inner exception:{inner}.", ex.InnerException);
                Disconnect();
                return 0;
            }
        }


        public static Task<int> SaveRecipe(List<RecipeFullWeb> recipesWeb)
        {
            List<RecipeDB> recipeDBs = new List<RecipeDB>();
            foreach (RecipeFullWeb web in recipesWeb)
            {
                recipeDBs.Add(new RecipeDB(web));
            }
            return SaveRecipes(recipeDBs);
        }

        /// <summary>
        /// Saves the recipes in the list of RecipeDB (DB-format)
        /// </summary>
        /// <param name="recipesToSave"></param> List of recipes to save in DB format
        /// <returns></returns>
        internal static async Task<int> SaveRecipes(List<RecipeDB> recipesToSave)
        {
            if (recipesToSave == null || recipesToSave.Count == 0)
            {
                Log.Error("Trying to save an empty list of recipes.");
                return 0;
            }
            else
            {
                HashSet<int> recipeIDList = new HashSet<int>();
                foreach (RecipeDB recipe in recipesToSave)
                {
                    if (recipe != null && recipe.recipe_id != 0)
                    {
                        recipeIDList.Add(recipe.recipe_id);
                    }
                }
                try
                {
                    using (GilGoblinDbContext context = getContext())
                    {

                        List<RecipeDB> existentRecipes = new List<RecipeDB>();

                        List<ItemDB> itemsDB = context.data
                                .Where(t => t.fullRecipes.All(
                                      (x => recipeIDList.Contains(x.recipe_id))))
                                .ToList();
                        if (itemsDB == null)
                        {
                            // None found, save everything
                            await context.AddRangeAsync(recipesToSave);
                            existentRecipes.Clear();
                        }
                        else
                        {
                            //Existent entries can be added to the tracker
                            //TODO: improve this... with LINQ?
                            foreach (ItemDB existentDBEntry in itemsDB)
                            {
                                existentRecipes.AddRange(existentDBEntry.fullRecipes);
                                foreach (RecipeDB existentRecipe in existentDBEntry.fullRecipes)
                                {
                                    context.Update(existentRecipe);
                                }
                            }

                            IEnumerable<RecipeDB> saveMeList
                                = recipesToSave.Except(existentRecipes);
                            //Non-existent entries are added to the tracker
                            foreach (RecipeDB saveRecipe in saveMeList)
                            {
                                if (saveRecipe != null)
                                {
                                    await context.AddAsync<RecipeDB>(saveRecipe);
                                }
                            }
                        }

                        return await context.SaveChangesAsync(); ;
                    }
                }
                catch (Exception ex) when (ex is SqliteException || ex is InvalidOperationException)
                {
                    //Maybe the database doesn't exist yet or not found
                    //Either way, we can return null -> it is not on the database

                    return 0;
                }
                catch (Exception ex)
                {

                    Log.Error("Exception: {Message}. Inner: {Inner}", ex.Message, ex.InnerException);
                    Disconnect();
                    return 0; ;
                }
            }
        }
    }
}




