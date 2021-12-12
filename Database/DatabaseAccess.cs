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


        public static void createTableMarketData(SqliteConnection conn, MarketData marketData)
        {
            using (SqliteCommand query = conn.CreateCommand())
            {
                try
                {
                    MarketDataContext marketDataContext = new MarketDataContext();
                    marketDataContext.Database.EnsureCreated();
                    Disconnect(conn);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    Disconnect(conn);
                }
            }
        }

        public static bool SaveMarketDataDB(MarketData marketData)
        {
            using (SqliteConnection conn = Connect())
            {
                try
                {
                    createTableMarketData(conn, marketData);
                    MarketDataContext marketDataContext = new MarketDataContext();                    
                    marketDataContext.Add<MarketDataDB>(new MarketDataDB(marketData));

                    marketDataContext.SaveChanges();

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
                        //query.CommandText =
                        //    @"
                        //        SELECT * 
                        //        FROM marketData
                        //        WHERE item_id = $item_id
                        //        LIMIT 1";
                        //using (var reader = query.ExecuteReader())
                        //{
                        //    reader.Read();
                        //    //MarketData marketData// = reader.CreateObjRef(Type.GetType("MarketData"));
                        //    Disconnect(conn);
                        //    return null; //TODO map this later
                        //}
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

            #region Required
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
            #endregion

        }
    }
}




