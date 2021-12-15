using GilGoblin.Finance;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using GilGoblin.Functions;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace GilGoblin.Database
{
    internal class DatabaseAccess
    {
        public static string file_path = Path.GetDirectoryName(AppContext.BaseDirectory);
        public static string db_name = "GilGoblin.db";
        public static string _path = Path.Combine(file_path, db_name);

        public static SqliteConnection conn;

        public static SqliteConnection Connect()
        {
            try
            {                
                if (conn == null)
                {
                    conn = new SqliteConnection("Data Source=" + _path);
                }

                //Already open, return
                if (conn.State == System.Data.ConnectionState.Open)
                {                   
                    return conn;
                }

                conn.Open();
                if (conn.State == System.Data.ConnectionState.Open)
                {
                    return conn;
                }
                else
                {
                    Console.WriteLine("Connection not open. State is: " + conn.State.ToString());
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
            if (DatabaseAccess.conn != null &&
            DatabaseAccess.conn.State != System.Data.ConnectionState.Closed)
            {
                try
                {
                    conn.Close();
                    conn.Dispose();
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }

        }

        public static async Task<int> SaveMarketDataDB(MarketData marketData)
        {
                try
                {
                MarketDataContext marketDataContext = new MarketDataContext();
                await marketDataContext.Database.EnsureCreatedAsync();

                MarketDataDB exists = marketDataContext.data
                    .FindAsync(marketData.item_id, marketData.world_id).GetAwaiter().GetResult();

                if (exists == null)
                {
                    MarketDataDB newDBEntry = new MarketDataDB(marketData);
                    await marketDataContext.AddAsync<MarketDataDB>(newDBEntry);
                }
                else
                {
                    exists.last_updated = marketData.last_updated;
                    exists.average_Price = marketData.average_Price;
                    marketDataContext.Update<MarketDataDB>(exists);
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

        // Read from DB: Doesn't work yet
        public static MarketData GetMarketDataDB(int item_id)
        {
            return null;
        }

        public class MarketDataContext : DbContext
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
                modelBuilder.Entity<MarketDataDB>().Property(t => t.average_Price);
                modelBuilder.Entity<MarketDataDB>().HasKey(t => new { t.item_id, t.world_id });

                modelBuilder.Entity<MarketDataDB>().HasMany(t => t.listings);
                //.WithOne().

                //modelBuilder.Entity<MarketListingDB>().Map( )
            }

        }

        public class MarketListingContext : DbContext
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




