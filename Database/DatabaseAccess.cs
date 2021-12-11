using GilGoblin.Finance;
using Microsoft.Data.Sqlite;
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

        public static bool saveMarketData(MarketData marketData)
        {
            using (SqliteConnection conn = Connect())
            {
                conn.Database.
            }
        }
    }
}
