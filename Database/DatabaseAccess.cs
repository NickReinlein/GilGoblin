﻿using GilGoblin.Finance;
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
            catch (Exception ex)
            {
                Log.Debug("Database startup did not succed: {message}", ex.Message);
                return;
            }
        }


        public static void Save()
        {
            try
            {
                ItemDBContext context = new ItemDBContext();
                context.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Debug("Database save did not succed! Message: {message}", ex.Message);
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
                    HashSet<int> itemIDList = new HashSet<int>();
                    foreach (MarketDataDB marketData in marketDataList)
                    {
                        if (marketData != null)
                        {
                            itemIDList.Add(marketData.itemID);
                        }
                    }

                    if (itemIDList.Count == 0 )
                    {
                        Log.Error("Could not save market data due to missing item ID and world ID list.");
                        return 0;
                    }

                    ItemDBContext context = new ItemDBContext();
                    
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

                ItemDB itemExists = ItemDBContext.data
                    .FindAsync(marketData.itemID, marketData.worldID).GetAwaiter().GetResult();

                if (itemExists == null)
                {
                    //New entry, add to entity tracker
                    await ItemDBContext.AddAsync<MarketDataDB>(marketData);
                }
                else
                {
                    // TODO: verify if this match works with object
                    // and find in list
                    MarketDataDB exists = itemExists.marketData
                        .Find(t => t.itemID == marketData.itemID &&
                                   t.worldID == marketData.worldID);
                    //Existing entry
                    exists.lastUpdated = DateTime.Now;
                    exists.averagePrice = marketData.averagePrice;
                    exists.listings = marketData.listings;
                    ItemDBContext.Update<ItemDB>(itemExists);
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





        public static Task<int> SaveRecipe(List<RecipeFullWeb> recipesWeb)
        {
            List<RecipeDB> recipeDBs = new List<RecipeDB>();
            foreach (RecipeFullWeb web in recipesWeb)
            {
                recipeDBs.Add(new RecipeDB(web));
            }
            return SaveRecipes(recipeDBs);
        }

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
                    ItemDBContext context = new ItemDBContext();
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
                                await context.AddAsync<RecipeDB>(existentRecipe);
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



