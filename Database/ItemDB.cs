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

    internal class ItemDBContext : DbContext
    {
        public DbSet<ItemDB> data { get; set; }
        public DbSet<MarketDataDB> marketData { get; set; }
        public DbSet<ItemInfoDB> itemInfoData { get; set; }
        public DbSet<RecipeDB> recipeData { get; set; }

        private SqliteConnection conn;

        public ItemDBContext()
            : base(new DbContextOptionsBuilder<ItemDBContext>().UseSqlite(DatabaseAccess.Connect()).Options)
        {
            conn = DatabaseAccess.Connect();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            this.conn = DatabaseAccess.Connect();
            optionsBuilder.UseSqlite(this.conn);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            /* General data for an item ID -- A main accessor class */
            modelBuilder.Entity<ItemDB>().ToTable("ItemDB");
            modelBuilder.Entity<ItemDB>().HasKey(t => t.itemID);
            modelBuilder.Entity<ItemDB>().HasOne(t => t.itemInfo);
            modelBuilder.Entity<ItemDB>().HasMany(t => t.marketData);
            modelBuilder.Entity<ItemDB>().HasMany(t => t.fullRecipes);


            /* General market data for an item ID & world (with calculated average price and more) */
            modelBuilder.Entity<MarketDataDB>().ToTable("MarketData");
            modelBuilder.Entity<MarketDataDB>().Property(t => t.itemID);
            modelBuilder.Entity<MarketDataDB>().Property(t => t.worldID);
            modelBuilder.Entity<MarketDataDB>().Property(t => t.lastUpdated);
            modelBuilder.Entity<MarketDataDB>().Property(t => t.averagePrice);
            modelBuilder.Entity<MarketDataDB>().HasKey(t => new { t.itemID, t.worldID });
            modelBuilder.Entity<MarketDataDB>().HasMany(t => t.listings);

            // All the market listings for an item & world ID
            modelBuilder.Entity<MarketListingDB>().ToTable("MarketListing");
            modelBuilder.Entity<MarketListingDB>().Property(t => t.Id).ValueGeneratedOnAdd();
            modelBuilder.Entity<MarketListingDB>().Property(t => t.item_id).IsRequired();
            modelBuilder.Entity<MarketListingDB>().Property(t => t.world_id).IsRequired();
            modelBuilder.Entity<MarketListingDB>().Property(t => t.timestamp);
            modelBuilder.Entity<MarketListingDB>().Property(t => t.price).IsRequired();
            modelBuilder.Entity<MarketListingDB>().Property(t => t.hq).IsRequired();
            modelBuilder.Entity<MarketListingDB>().Property(t => t.qty).IsRequired();

            //// Item info from the db with full recipes
            modelBuilder.Entity<ItemInfoDB>().ToTable("ItemInfoDB");
            modelBuilder.Entity<ItemInfoDB>().HasKey(t => t.itemID);
            modelBuilder.Entity<ItemInfoDB>().Property(t => t.name);
            modelBuilder.Entity<ItemInfoDB>().Property(t => t.iconID);
            modelBuilder.Entity<ItemInfoDB>().Property(t => t.description);
            modelBuilder.Entity<ItemInfoDB>().Property(t => t.vendor_price);
            modelBuilder.Entity<ItemInfoDB>().Property(t => t.stack_size);
            modelBuilder.Entity<ItemInfoDB>().Property(t => t.gatheringID);
            modelBuilder.Entity<ItemInfoDB>().HasMany(t => t.fullRecipes);


            // Database format for the full recipes
            modelBuilder.Entity<RecipeDB>().ToTable("RecipeDB");
            modelBuilder.Entity<RecipeDB>().HasKey(t => t.recipe_id);
            modelBuilder.Entity<RecipeDB>().Property(t => t.result_quantity);
            modelBuilder.Entity<RecipeDB>().Property(t => t.icon_id);
            modelBuilder.Entity<RecipeDB>().Property(t => t.target_item_id);
            modelBuilder.Entity<RecipeDB>().Property(t => t.CanHq);
            modelBuilder.Entity<RecipeDB>().Property(t => t.CanQuickSynth);
            modelBuilder.Entity<RecipeDB>().HasMany(t => t.ingredients);


            // The full recipe from the web
            modelBuilder.Entity<RecipeFullWeb>().ToTable("RecipeFullWeb");
            modelBuilder.Entity<RecipeFullWeb>().HasKey(t => t.recipe_id);
            modelBuilder.Entity<RecipeFullWeb>().Property(t => t.result_quantity).IsRequired();
            modelBuilder.Entity<RecipeFullWeb>().Property(t => t.icon_id);
            modelBuilder.Entity<RecipeFullWeb>().Property(t => t.target_item_id).IsRequired();
            modelBuilder.Entity<RecipeFullWeb>().Property(t => t.CanHq);
            modelBuilder.Entity<RecipeFullWeb>().Property(t => t.CanQuickSynth);
            modelBuilder.Entity<RecipeFullWeb>().HasMany(t => t.ingredients);


            // Item info from the web with short recipes
            modelBuilder.Entity<ItemInfoWeb>().ToTable("ItemInfoWeb");
            modelBuilder.Entity<ItemInfoWeb>().Property(t => t.itemID).IsRequired();
            modelBuilder.Entity<ItemInfoWeb>().Property(t => t.name);
            modelBuilder.Entity<ItemInfoWeb>().Property(t => t.iconID);
            modelBuilder.Entity<ItemInfoWeb>().Property(t => t.description);
            modelBuilder.Entity<ItemInfoWeb>().Property(t => t.vendor_price);
            modelBuilder.Entity<ItemInfoWeb>().Property(t => t.stack_size);
            modelBuilder.Entity<ItemInfoWeb>().Property(t => t.gatheringID);
            modelBuilder.Entity<ItemInfoWeb>().HasMany(t => t.recipeHeader);

            //The abbreviated recipe with only 3 properties in the market data
            modelBuilder.Entity<ItemRecipeHeaderAPI>().ToTable("ItemRecipeHeaderAPI");
            modelBuilder.Entity<ItemRecipeHeaderAPI>().Property(t => t.ID).ValueGeneratedOnAdd(); ;
            modelBuilder.Entity<ItemRecipeHeaderAPI>().Property(t => t.recipe_id).IsRequired();
            modelBuilder.Entity<ItemRecipeHeaderAPI>().Property(t => t.class_job_id).IsRequired();
            modelBuilder.Entity<ItemRecipeHeaderAPI>().Property(t => t.level).IsRequired();
        }
    }
}
