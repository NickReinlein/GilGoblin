using GilGoblin.Finance;
using GilGoblin.WebAPI;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static GilGoblin.Database.RecipeDB;

namespace GilGoblin.Database
{
    internal class DatabaseAccess
    {
        public static string _file_path = Path.GetDirectoryName(AppContext.BaseDirectory);
        public static string _db_name = "GilGoblin.db";
        public static string _path = Path.Combine(_file_path, _db_name);

        public static SqliteConnection _conn;

        internal static SqliteConnection Connect()
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
                    Console.Write(ex.Message);
                }
            }

        }

        public static void Startup()
        {
            try
            {
                ItemDBContext ItemDBContext = new ItemDBContext();
                ItemDBContext.Database.EnsureCreatedAsync();
            }
            catch(Exception ex)
            {
                Log.Debug("Database startup did not succed: {message}", ex.Message);
                return;
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
                    ItemDBContext ItemDBContext = new ItemDBContext();

                    //TODO!
                    List<MarketDataDB> exists = new List<MarketDataDB>();
                    //List<MarketDataDB> exists = ItemDBContext.data
                    //        .Where(t => (t.marketData.world_id == t.world_id && 
                    //                     t.item_id == t.item_id))
                    //        .Include(t => t.listings)
                    //        .ToList();


                    if (exists == null)
                    {
                        //New entries, add to entity tracker     
                        await ItemDBContext.AddRangeAsync(marketDataList);
                    }
                    else
                    {
                        //Existing entries get updated
                        foreach (MarketDataDB exist in exists)
                        {
                            //Existing entry
                            exist.average_price = exist.average_price;
                            exist.listings = exist.listings;
                            ItemDBContext.Update<MarketDataDB>(exist);
                        }
                        //Non-existent entries are added to the tracker
                        foreach (MarketDataDB newData in marketDataList)
                        {
                            var thisExists = exists
                                .Find(t => t.item_id == newData.item_id &&
                                           t.world_id == newData.world_id);
                            if (thisExists == null)
                            {
                                await ItemDBContext.AddAsync<MarketDataDB>(newData);
                            }
                        }
                    }

                    return await ItemDBContext.SaveChangesAsync(); ;

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
                ItemDBContext ItemDBContext = new ItemDBContext();

                ItemDB exists = ItemDBContext.data
                    .FindAsync(marketData.item_id, marketData.world_id).GetAwaiter().GetResult();

                if (exists == null)
                {
                    //New entry, add to entity tracker
                    await ItemDBContext.AddAsync<MarketDataDB>(marketData);
                }
                else
                {
                    //Existing entry
                    exists.marketData.last_updated = DateTime.Now;
                    exists.marketData.average_price = marketData.average_price;
                    exists.marketData.listings = marketData.listings;
                    ItemDBContext.Update<ItemDB>(exists);
                }
                int success = 0;
                if (saveToDB) { success = await ItemDBContext.SaveChangesAsync(); }
                return success;
            }
            catch (Exception ex)
            {
                Log.Error("Exception: {message}.", ex.Message);
                Log.Error("Inner exception:{inner}.", ex.InnerException);
                Disconnect();
                return 0;
            }
        }





        public static Task<int> SaveRecipes(List<RecipeFullWeb> recipesWeb)
        {
            List<RecipeDB> recipeDBs = new List<RecipeDB>();
            foreach (RecipeFullWeb web in recipesWeb)
            {
                recipeDBs.Add(new RecipeDB(web));
            }
            return SaveRecipes(recipeDBs);
        }

        internal static async Task<int> SaveRecipes(List<RecipeDB> recipes)
        {
            if (recipes == null || recipes.Count == 0)
            {
                Log.Error("Trying to save an empty list of recipes.");
                return 0;
            }
            else
            {
                HashSet<int> recipeIDList = new HashSet<int>();
                foreach (RecipeDB recipe in recipes)
                {
                    if (recipe != null && recipe.recipe_id != 0)
                    {
                        recipeIDList.Add(recipe.recipe_id);
                    }
                }
                try
                {
                    ItemDBContext context = new ItemDBContext();
                    await context.Database.EnsureCreatedAsync();

                    var test = context.data
                            .Where(t => t.recipes.All(
                                  (x => recipeIDList.Contains(x.recipe_id))))
                            .ToList();

                        //= context.data
                    //        .Where(t => t.fullRecipes.All(
                    //              (x => recipeIDList.Contains(x.recipe_id))))
                    //        .ToList();
                    if (test == null)
                    //if (exists == null)
                    {
                        await context.AddRangeAsync(recipes);
                    }
                    else
                    {
                        //Non-existent entries are added to the tracker
                        foreach (RecipeDB newData in recipes)
                        {
                            RecipeDB thisExists = null;
                            try
                            {
                                //thisExists = exists
                                //    .Find(t => t.recipe_id == newData.recipe_id);
                            }
                            catch (Exception) { thisExists = null; }

                            if (thisExists == null)
                            {
                                await context.AddAsync<RecipeDB>(newData);
                            }
                            else { } //No need to update recipes; they never change
                        }
                    }

                    return await context.SaveChangesAsync(); ;

                }
                catch (Exception ex)
                {
                    if (ex is Microsoft.Data.Sqlite.SqliteException ||
                        ex is System.InvalidOperationException)
                    {
                        //Maybe the database doesn't exist yet or not found
                        //Either way, we can return null -> it is not on the database
                        
                        return 0;
                    }
                    else
                    {

                        Log.Error("Exception: {message}.", ex.Message);
                        Log.Error("Inner exception:{inner}.", ex.InnerException);
                        Disconnect();
                        return 0; ;
                    }
                }
            }
        }
    }
}




