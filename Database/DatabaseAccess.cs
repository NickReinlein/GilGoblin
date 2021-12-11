using Microsoft.Data.Sqlite;
using System;
using System.IO;

namespace GilGoblin.Database
{
    internal class DatabaseAccess
    {
        public static string file_path = Path.GetDirectoryName(AppContext.BaseDirectory);
        public static string db_name = "GilGoblin.db";

        public static bool Connect()
        {
            try
            {
                //string file_path = Path.GetDirectoryName(AppContext.BaseDirectory);
                //string db_name = "gilgoblin.db";
                string full_path = Path.Combine(file_path, db_name);
                Console.WriteLine(full_path);
                SqliteConnection conn = new SqliteConnection("Data Source="+full_path);
                conn.Open();

                if (conn.State == System.Data.ConnectionState.Open)
                {
                    Console.WriteLine("Open Connection");
                }
                else
                {
                    Console.WriteLine("Connection not open. State is: " + conn.State.ToString());
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("failed connection: " + ex.Message);                
                return false;
            }
            return true;
        }
    }
}
