using Microsoft.Data.Sqlite;
using Serilog;

namespace GilGoblin.Database;

public class GoblinDatabase
{
    public static string _file_path = Path.GetDirectoryName(AppContext.BaseDirectory);
    public static string _db_name = "GilGoblin.db";
    public static string _path = Path.Combine(_file_path, _db_name);
    private static SqliteConnection? Connection { get; set; }

    public GilGoblinDbContext GetContext()
    {
        var context = new GilGoblinDbContext();
        if (context.ItemInfo?.Count() < 10) { 
            FillTableItemInfo();
        }
        return context;
    }

    private void FillTableItemInfo()
    {
        throw new NotImplementedException();
    }

    public static SqliteConnection? Connect()
    {
        try
        {
            Connection ??= new SqliteConnection("Data Source=" + _path);

            if (Connection.State == System.Data.ConnectionState.Open)
                return Connection;

            Connection.Open();
            if (Connection.State == System.Data.ConnectionState.Open)
                return Connection;

            Log.Error("Connection not open. State is: {State}.", Connection.State);
            return null;
        }
        catch (Exception ex)
        {
            Log.Error("failed connection:{Message}.", ex.Message);
            return null;
        }
    }

    public static void Disconnect()
    {
        if (GoblinDatabase.Connection.State == System.Data.ConnectionState.Closed)
            return;

        Connection.Close();
        Connection.Dispose();
    }

    public static void Save(GilGoblinDbContext context)
    {
        try
        {
            Log.Debug("Saving to database.");

            context.Database.EnsureCreated();
            var savedEntries = context.SaveChanges();

            Log.Debug("Saved {saved} entries to the database.", savedEntries);
            context.Dispose();
        }
        catch (Exception ex)
        {
            Log.Error("Database save failed! Message: {Message}.", ex.Message);
        }
    }
}
