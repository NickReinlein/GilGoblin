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
                    MarketDataContext marketDataContext = new MarketDataContext();
                    await marketDataContext.Database.EnsureCreatedAsync();

                    List<MarketDataDB> exists = marketDataContext.data
                            .Where(t => (t.world_id == t.world_id && t.item_id == t.item_id))
                            .Include(t => t.listings)
                            .ToList();


                    if (exists == null)
                    {
                        //New entry, add to entity tracker
                        //await marketDataContext.AddAsync<MarketDataDB>(marketData);)        
                        await marketDataContext.AddRangeAsync(marketDataList);
                    }
                    else
                    {
                        //Existing entries get updated
                        foreach (MarketDataDB exist in exists)
                        {
                            //Existing entry
                            exist.average_price = exist.average_price;
                            exist.listings = exist.listings;
                            marketDataContext.Update<MarketDataDB>(exist);
                        }
                        //Non-existent entries are added to the tracker
                        foreach (MarketDataDB newData in marketDataList)
                        {
                            var thisExists = exists
                                .Find(t => t.item_id == newData.item_id &&
                                           t.world_id == newData.world_id);
                            if (thisExists == null)
                            {
                                await marketDataContext.AddAsync<MarketDataDB>(newData);
                            }
                        }
                    }

                    return await marketDataContext.SaveChangesAsync(); ;

                }
                catch (Exception ex)
                {
                    Log.Error("Exception: {message}.", ex.Message);
                    Log.Error("Inner exception: {innser}.", ex.InnerException);
                    Disconnect();
                    return 0;
                }
            }
        }

        internal static async Task<int> SaveMarketDataSingle(MarketDataDB marketData, bool saveToDB = true)
        {
            try
            {
                MarketDataContext marketDataContext = new MarketDataContext();
                await marketDataContext.Database.EnsureCreatedAsync();

                MarketDataDB exists = marketDataContext.data
                    .FindAsync(marketData.item_id, marketData.world_id).GetAwaiter().GetResult();

                if (exists == null)
                {
                    //New entry, add to entity tracker
                    await marketDataContext.AddAsync<MarketDataDB>(marketData);
                }
                else
                {
                    //Existing entry
                    exists.last_updated = DateTime.Now;
                    exists.average_price = marketData.average_price;
                    exists.listings = marketData.listings;
                    marketDataContext.Update<MarketDataDB>(exists);
                }
                int success = 0;
                if (saveToDB) { success = await marketDataContext.SaveChangesAsync(); }
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

        /// <summary>
        /// Searches the database for a bulk list of items given their ID & world
        /// </summary>
        /// <param name="itemIDList"></param>A List of integers to represent item ID
        /// <param name="world_id"></param>the world ID for world-specific data (ie: market price)
        /// <returns></returns>
        public static List<MarketDataDB> GetMarketDataDBBulk(List<int> itemIDList, int world_id)
        {
            try
            {
                MarketDataContext marketDataContext = new MarketDataContext();

                List<MarketDataDB> exists = marketDataContext.data
                        .Where(t => (t.world_id == world_id && itemIDList.Contains(t.item_id)))
                        .Include(t => t.listings)
                        .ToList();
                return exists;
            }
            catch (Exception ex)
            {
                if (ex is Microsoft.Data.Sqlite.SqliteException ||
                    ex is System.InvalidOperationException)
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
                    Disconnect();
                    return null; ;
                }
            }
        }

        /// <summary>
        /// Searches the database for MarketData (average market price, vendor price, etc)
        /// </summary>
        /// <param name="item_id"></param>item ID (ie: 5057 for Copper Ingot)
        /// <param name="world_id"></param>world ID (ie: 34 for Brynhildr)
        /// <returns></returns>
        public static MarketDataDB GetMarketDataDB(int item_id, int world_id)
        {
            try
            {
                MarketDataContext marketDataContext = new MarketDataContext();

                MarketDataDB exists = marketDataContext.data
                        .Where(t => (t.item_id == item_id && t.world_id == world_id))
                        .Include(t => t.listings)
                        .FirstOrDefault();

                return exists;
            }
            catch (Exception ex)
            {
                if (ex is Microsoft.Data.Sqlite.SqliteException ||
                    ex is System.InvalidOperationException)
                {
                    //Maybe the database doesn't exist yet or not found
                    //Either way, we can return null -> it is not on the database
                    return null;
                }
                else
                {

                    Log.Error("Exception: {message}.", ex.Message);
                    Log.Error("Inner exception:{inner}.", ex.InnerException);
                    Disconnect();
                    return null; ;
                }
            }
        }

        public static Task<int> SaveRecipes(List<RecipeWeb> recipesWeb)
        {
            List<RecipeDB> recipeDBs = new List<RecipeDB>();
            foreach (RecipeWeb web in recipesWeb)
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
                    RecipeContext context = new RecipeContext();
                    await context.Database.EnsureCreatedAsync();

                    List<RecipeDB> exists = context.data
                            .Where(t => recipeIDList.Contains(t.recipe_id))
                            .ToList();
                    if (exists == null)
                    {
                        await context.AddRangeAsync(recipes);
                    }
                    else
                    {
                        //Non-existent entries are added to the tracker
                        foreach (RecipeDB newData in recipes)
                        {
                            RecipeDB thisExists;
                            try
                            {
                                thisExists = exists
                                    .Find(t => t.recipe_id == newData.recipe_id);
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
                    Log.Error("Exception: {message}.", ex.Message);
                    Log.Error("Inner exception:{inner}.", ex.InnerException);
                    Disconnect();
                    return 0;
                }
            }
        }

        internal class MarketDataContext : DbContext
        {
            public DbSet<MarketDataDB> data { get; set; }
            private SqliteConnection conn;

            public MarketDataContext()
                : base(new DbContextOptionsBuilder<MarketDataContext>().UseSqlite(Connect()).Options)
            {
                conn = Connect();
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                this.conn = Connect();
                optionsBuilder.UseSqlite(this.conn);
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<MarketDataDB>().ToTable("MarketData");
                modelBuilder.Entity<MarketDataDB>().Property(t => t.item_id);
                modelBuilder.Entity<MarketDataDB>().Property(t => t.world_id);
                modelBuilder.Entity<MarketDataDB>().Property(t => t.last_updated);
                modelBuilder.Entity<MarketDataDB>().Property(t => t.average_price);
                modelBuilder.Entity<MarketDataDB>().HasKey(t => new { t.item_id, t.world_id });
                modelBuilder.Entity<MarketDataDB>().HasMany(t => t.listings);
                modelBuilder.Entity<MarketDataDB>().HasMany(t => t.recipes);
            }

        }

        internal class MarketListingContext : DbContext
        {
            public DbSet<MarketListingDB> listingDB { get; set; }
            private SqliteConnection conn;

            public MarketListingContext()
                : base(new DbContextOptionsBuilder<MarketListingContext>().UseSqlite(Connect()).Options)
            {
                conn = Connect();
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                this.conn = Connect();
                optionsBuilder.UseSqlite(this.conn);
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<MarketListingDB>().ToTable("MarketListing");
                modelBuilder.Entity<MarketListingDB>().Property(t => t.Id).ValueGeneratedOnAdd();
                modelBuilder.Entity<MarketListingDB>().Property(t => t.item_id).IsRequired();
                modelBuilder.Entity<MarketListingDB>().Property(t => t.world_id).IsRequired();
                modelBuilder.Entity<MarketListingDB>().Property(t => t.timestamp);
                modelBuilder.Entity<MarketListingDB>().Property(t => t.price).IsRequired();
                modelBuilder.Entity<MarketListingDB>().Property(t => t.hq).IsRequired();
                modelBuilder.Entity<MarketListingDB>().Property(t => t.qty).IsRequired();
            }
        }

        internal class RecipeContext : DbContext
        {
            public DbSet<RecipeDB> data { get; set; }
            private SqliteConnection conn;

            public RecipeContext()
                : base(new DbContextOptionsBuilder<RecipeContext>().UseSqlite(Connect()).Options)
            {
                conn = Connect();
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                this.conn = Connect();
                optionsBuilder.UseSqlite(this.conn);
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<RecipeDB>()
                    .ToTable("Recipe");
                modelBuilder.Entity<RecipeDB>()
                    .HasKey(t => t.recipe_id);
                modelBuilder.Entity<RecipeDB>()
                    .Property(t => t.result_quantity).IsRequired();
                modelBuilder.Entity<RecipeDB>()
                    .Property(t => t.icon_id);
                modelBuilder.Entity<RecipeDB>()
                    .Property(t => t.target_item_id).IsRequired();
                modelBuilder.Entity<RecipeDB>()
                    .Property(t => t.CanHq);
                modelBuilder.Entity<RecipeDB>()
                    .Property(t => t.CanQuickSynth);
                modelBuilder.Entity<RecipeDB>()
                    .HasMany(t => t.ingredients);
            }

        }
    }
}




