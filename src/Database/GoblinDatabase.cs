using Microsoft.Data.Sqlite;
using Serilog;
using GilGoblin.Services;
using GilGoblin.Pocos;

namespace GilGoblin.Database;

public class GoblinDatabase
{
    public static readonly string ResourceFilePath = System.IO.Path.Combine(
        Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).Parent.Parent.FullName,
        "Resources/AllItems.json"
    );
    public static string FilePath = System.IO.Path.GetDirectoryName(AppContext.BaseDirectory);
    public static string DbName = "GilGoblin.db";
    public static string Path = System.IO.Path.Combine(FilePath, DbName);
    private static SqliteConnection? Connection { get; set; }

    public GilGoblinDbContext GetContext()
    {
        var context = new GilGoblinDbContext();
        FillTablesIfEmpty(context);
        return context;
    }

    private async Task FillTablesIfEmpty(GilGoblinDbContext context)
    {
        if (context.ItemInfo?.Count() < 10)
        {
            var result = await JSONLoader.LoadJSONFile<ItemInfoPoco>(ResourceFilePath);
            if (result.Any())
            {
                // save results if correct
            }
        }
    }

    public static SqliteConnection? Connect()
    {
        try
        {
            Connection ??= new SqliteConnection("Data Source=" + Path);

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
