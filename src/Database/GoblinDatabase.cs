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
        Connection ??= Connect();
        if (Connection is null)
            return null;

        var context = new GilGoblinDbContext(Connection);
        await FillTablesIfEmpty(context);
        return context;
    }

    private static async Task FillTablesIfEmpty(GilGoblinDbContext context)
    {
        var count = context.ItemInfo?.Count();
        if (count < 10)
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
        if (Connection is not null)
            return Connection;

        try
        {
            var path = ResourceFilePath(DbName);
            Connection ??= new SqliteConnection("Data Source=" + path);

            if (Connection.State == System.Data.ConnectionState.Open)
                return Connection;

            Connection.Open();
            if (Connection.State == System.Data.ConnectionState.Open)
                return Connection;

            throw new Exception($"Connection not open. State is: {Connection?.State}");
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
        Directory.GetParent(System.IO.Directory.GetCurrentDirectory()).FullName,
        "resources/"
    );

    public static string ResourceFilePath(string resourceFileName) =>
        System.IO.Path.Combine(ResourcesFolderPath, resourceFileName);

    public const string ResourceFileNameItemCsv = "Item.csv";
    public const string DbName = "GilGoblin.db";
}
