using GilGoblin.Finance;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

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
                    Console.WriteLine("Connection not open. State is: " + _conn.State.ToString());
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed connection: " + ex.Message);
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
                Console.WriteLine("Trying to save an empty list of market data.");
                return 0;
            }
            else
            {
                int saves = 0;
                foreach (MarketDataDB marketData in marketDataList ?? Enumerable.Empty<MarketDataDB>())
                {
                    saves += await SaveMarketData(marketData);
                }
                return saves;
            }
        }

        internal static async Task<int> SaveMarketData(MarketDataDB marketData)
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
                    //Check for freshness: if old, update data
                    TimeSpan diff = DateTime.Now - marketData.last_updated;
                    double hoursElapsed = diff.TotalHours;
                    if (hoursElapsed > MarketData._staleness_hours_for_refresh)
                    {
                        exists.last_updated = DateTime.Now;
                        exists.average_Price = marketData.average_Price;
                        exists.listings = marketData.listings;
                        marketDataContext.Update<MarketDataDB>(exists);
                    }
                }
                int success = await marketDataContext.SaveChangesAsync();
                return success;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
                Console.WriteLine("Inner exception: " + ex.InnerException);
                Disconnect();
                return 0;
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

                    Console.WriteLine("Exception: " + ex.Message);
                    Console.WriteLine("Inner exception: " + ex.InnerException);
                    Disconnect();
                    return null; ;
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
                modelBuilder.Entity<MarketDataDB>().ToTable("MarketDataDB");
                modelBuilder.Entity<MarketDataDB>().Property(t => t.item_id);
                modelBuilder.Entity<MarketDataDB>().Property(t => t.world_id);
                modelBuilder.Entity<MarketDataDB>().Property(t => t.last_updated);
                modelBuilder.Entity<MarketDataDB>().Property(t => t.average_Price);
                modelBuilder.Entity<MarketDataDB>().HasKey(t => new { t.item_id, t.world_id });

                modelBuilder.Entity<MarketDataDB>().HasMany(t => t.listings);
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


    }
}




