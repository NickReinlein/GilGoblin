using GilGoblin.Finance;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;

namespace GilGoblin.Database
{
    internal class DatabaseAccess
    {
        public static string file_path = Path.GetDirectoryName(AppContext.BaseDirectory);
        public static string db_name = "GilGoblin.db";

        public static SqliteConnection Connect()
        {
            try
            {
                string full_path = Path.Combine(file_path, db_name);
                SqliteConnection conn = new SqliteConnection("Data Source=" + full_path);

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

        public static void Disconnect(SqliteConnection conn)
        {
            conn.Close();
        }

        public static bool SaveMarketDataDB(MarketData marketData)
        {
            using (SqliteConnection conn = Connect())
            {
                try
                {
                    MarketDataContext marketDataContext = new MarketDataContext();
                    marketDataContext.Database.EnsureCreated();
                    //marketDataContext.Add<MarketDataDB>(new MarketDataDB(marketData));
                    marketDataContext.Update<MarketDataDB>(new MarketDataDB(marketData));
                    marketDataContext.SaveChanges();

                    MarketListingContext marketListingContext = new MarketListingContext();
                    marketListingContext.Database.EnsureCreated();
                    foreach (MarketListing marketListing in marketData.listings)
                    {
                        marketDataContext.Add<MarketListingDB>(new MarketListingDB(marketListing));
                    }
                    marketListingContext.SaveChanges();

                    Disconnect(conn);
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Disconnect(conn);
                    return false;
                }

            }
        }

        // Doesn't work yet
        public static MarketData GetMarketDataDB(int item_id)
        {
            using (SqliteConnection conn = Connect())
            {
                using (SqliteCommand query = conn.CreateCommand())
                {
                    try
                    {
                        return null;
                    }
                    catch (Exception err)
                    {
                        Console.WriteLine(err.Message);
                        Disconnect(conn);
                        return null;
                    }
                }
            }
        }

        public partial class MarketDataContext : DbContext
        {
            public DbSet<MarketDataDB> marketDataDB { get; set; }

            public MarketDataContext() : base()
            { }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                string connectionString = Path.Combine(file_path, db_name);
                optionsBuilder.UseSqlite($"Data Source=" + connectionString);
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<MarketDataDB>().ToTable("MarketData");
                modelBuilder.Entity<MarketDataDB>().Property(t => t.item_id)
                                                   .IsRequired();
                modelBuilder.Entity<MarketDataDB>().Property(t => t.world_name)
                                                    .IsRequired();
                modelBuilder.Entity<MarketDataDB>().Property(t => t.last_updated)
                                                    .IsRequired();
                modelBuilder.Entity<MarketDataDB>().Property(t => t.average_Price);
            }
        }

        public partial class MarketListingContext : DbContext
        {
            public DbSet<MarketListingDB> listingDB { get; set; }

            public MarketListingContext() : base()
            { }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                string connectionString = Path.Combine(file_path, db_name);
                optionsBuilder.UseSqlite($"Data Source=" + connectionString);
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Entity<MarketListingDB>().ToTable("MarketListings");
                modelBuilder.Entity<MarketListingDB>().Property(t => t.item_id).IsRequired();
                modelBuilder.Entity<MarketListingDB>().Property(t => t.world_name).IsRequired();
                modelBuilder.Entity<MarketListingDB>().Property(t => t.timestamp).IsRequired();
                modelBuilder.Entity<MarketListingDB>().Property(t => t.price).IsRequired();
                modelBuilder.Entity<MarketListingDB>().Property(t => t.hq).IsRequired();
                modelBuilder.Entity<MarketListingDB>().Property(t => t.qty).IsRequired();
            }

        }
    }
}




