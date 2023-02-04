using Microsoft.Data.Sqlite;
using Serilog;
using GilGoblin.Services;
using GilGoblin.Pocos;

namespace GilGoblin.Database;

public class GoblinDatabase
{
    private static SqliteConnection? Connection { get; set; }

    public GoblinDatabase() { }

    public async static Task<GilGoblinDbContext?> GetContext()
    {
        Connection ??= await Connect();
        var context = new GilGoblinDbContext();
        await FillTablesIfEmpty(context);
        return context;
    }

    private static async Task FillTablesIfEmpty(GilGoblinDbContext context)
    {
        if (context.ItemInfo?.Count() < 10)
        {
            var path = ResourceFilePath(ResourceFileNameItemCsv);
            var result = CsvInteractor<ItemInfoPoco>.LoadFile(path);
            if (result.Any())
            {
                await context.AddRangeAsync(result);
                await context.SaveChangesAsync();
            }
        }
    }

    public static SqliteConnection? Connect()
    {
        try
        {
            var path = ResourceFilePath(DbName);
            Connection ??= new SqliteConnection("Data Source=" + path);

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
        Connection?.Close();
        Connection?.Dispose();
    }

    public static void Save(GilGoblinDbContext context)
    {
        try
        {
            Log.Debug("Saving to database.");

            var savedEntries = context.SaveChanges();

            Log.Debug("Saved {saved} entries to the database.", savedEntries);
            context.Dispose();
        }
        catch (Exception ex)
        {
            Log.Error("Database save failed! Message: {Message}.", ex.Message);
        }
    }

    public static readonly string ResourcesFolderPath = System.IO.Path.Combine(
        Directory
            .GetParent(System.IO.Directory.GetCurrentDirectory())
            .Parent.Parent.Parent.FullName,
        "resources/"
    );

    public static string ResourceFilePath(string resourceFileName) =>
        System.IO.Path.Combine(ResourcesFolderPath, resourceFileName);

    public const string ResourceFileNameItemCsv = "Item.csv";
    public const string DbName = "GilGoblin.db";
}
