using GilGoblin.Finance;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
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

            using (DbContext dbContext = new MarketDataContext()) {
                try
                {
                    dbContext.Database.EnsureCreated();
                    dbContext.Add(marketData);
                    dbContext.SaveChanges();
                    return true;
                }
                catch(Exception ex) 
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
            }

            //using (SqliteConnection conn = Connect())
            //{
            //    using (SqliteCommand query = conn.CreateCommand())
            //    {
            //        try
            //        {
            //            int item_id = marketData.item_id;
            //            query.Parameters.AddWithValue("@item_id", SqliteType.Integer);
            //            query.CommandText =
            //               @"
            //                    UPDATE MarketData
            //                    WHERE item_id = $item_id
            //                    LIMIT 1";
            //            query.ExecuteNonQuery();
            //            Disconnect(conn);
            //            return true;
            //        }
            //        catch (Exception ex)
            //        {
            //            Console.WriteLine(ex.Message);
            //            Disconnect(conn);
            //            return false;
            //        }
            //    }
            //}
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

        public class MarketDataContext : DbContext
        {
            public DbSet<MarketData> MarketData { get; set; }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                string connectionString = Path.Combine(file_path, db_name);
                optionsBuilder.UseSqlite($"Data Source=" + connectionString);
            }

            #region Required
            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                //modelBuilder.Entity<MarketData>()
                //    .Property(b => b.last_updated)
                //    .IsRequired();

                //modelBuilder.Entity<MarketData>()
                //                    .Property(p => p.average_Price)
                //                    .IsRequired();
            }
            #endregion

            //No suitable constructor found for entity type 'MarketData'.
            //The following constructors had parameters that could not be bound to properties
            //of the entity type: cannot bind 'itemID', 'worldName', 'lastUploadTime',
            //'entries' in 'MarketData(int itemID, string worldName, long lastUploadTime,
            //List<MarketListing> entries)'.

        }
    }

}
