using Microsoft.Data.Sqlite;
using Serilog;
using GilGoblin.Services;
using GilGoblin.Pocos;
using System.Data;

namespace GilGoblin.Database;

public class GoblinDatabase
{
    private static SqliteConnection? Connection { get; set; }

    public GoblinDatabase() { }

    public async static Task<GilGoblinDbContext?> GetContext()
    {
        Connection ??= Connect();
        if (Connection is null || Connection.State != ConnectionState.Open)
            return null;

        var context = new GilGoblinDbContext(Connection);
        await FillTablesIfEmpty(context);
        return context;
    }

    private static async Task FillTablesIfEmpty(GilGoblinDbContext? context)
    {
        try
        {
            if (context is null || context.ItemInfo is null)
                return;
            await context.Database.EnsureCreatedAsync();

            if (context.ItemInfo.Count() < 10)
                await FillTable<ItemInfoPoco>(context);

            if (context.Recipe.Count() < 10)
                await FillTable<RecipePoco>(context);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }

    private static async Task FillTable<T>(GilGoblinDbContext context) where T : class
    {
        var pocoName = typeof(T).ToString().Split(".")[2];
        var tableName = pocoName.Remove(pocoName.Length - 4);
        Log.Warning(
            "Database table {TableName} has missing entries. Loading entries from Csv",
            tableName
        );

        var result = CsvInteractor<T>.LoadFile(ResourceFilePath(ResourceFilenameCsv(tableName)));
        if (result.Any())
        {
            // await context.AddRangeAsync(result);
            await context.SaveChangesAsync();
            Log.Information(
                "Sucessfully saved to table {TableName} {ResultCount} entries from Csv",
                tableName,
                result.Count()
            );
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

    public static string ResourceFilePath(string resourceFilename) =>
        System.IO.Path.Combine(ResourcesFolderPath, resourceFilename);

    public static string ResourceFilenameCsv(string filename) => string.Concat(filename, ".csv");

    public const string DbName = "GilGoblin.db";
}
