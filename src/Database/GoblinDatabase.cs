using Microsoft.Data.Sqlite;
using Serilog;
using GilGoblin.Services;
using GilGoblin.Pocos;
using System.Data;
using GilGoblin.Web;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Net.Http;
using System.Collections.Generic;
using System.IO;
using GilGoblin.Extensions;

namespace GilGoblin.Database;

public class GoblinDatabase
{
    private readonly IPriceDataFetcher _priceFetcher;
    private static SqliteConnection? Connection { get; set; }

    public GoblinDatabase(IPriceDataFetcher priceFetcher)
    {
        _priceFetcher = priceFetcher;
    }

    public async Task<GilGoblinDbContext?> GetContextAsync()
    {
        Connection ??= Connect();
        if (Connection is null || Connection.State != ConnectionState.Open)
            return null;

        await FillTablesIfEmpty();

        return GoblinDbContext;
    }

    private static GilGoblinDbContext GoblinDbContext => new(Connection ??= Connect());

    private async Task FillTablesIfEmpty()
    {
        try
        {
            using var context = GoblinDbContext;
            if (context is null || context.ItemInfo is null || context.Recipe is null)
                return;
            await context.Database.EnsureCreatedAsync();

            if (context.ItemInfo.Count() < 1000)
                await FillTable<ItemInfoPoco>();

            if (context.Recipe.Count() < 1000)
                await FillTable<RecipePoco>();

            if (context.Price?.Count() < 1000)
                await FillTable<PricePoco>();
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }

    private async Task FetchPrices()
    {
        LogTaskStart<PriceWebPoco>("Fetching prices from API");

        var batches = await _priceFetcher.GetAllIDsAsBatchJobsAsync(TestWorldID);

        foreach (var batch in batches)
        {
            var timer = new Stopwatch();
            timer.Start();
            var result = await _priceFetcher.FetchMultiplePricesAsync(TestWorldID, batch);

            if (!result.Any())
                throw new HttpRequestException("Failed to fetch prices from Universalis API");

            await SaveBatchResult(result);

            timer.Stop();
            Log.Information("Total time for batch in ms: {Elapsed}", timer.ElapsedMilliseconds);
            await Task.Delay(ApiSpamPreventionDelayInMS);
        }
    }

    private static async Task SaveBatchResult(IEnumerable<PriceWebPoco?> result)
    {
        using var context = GoblinDbContext;

        var pricesToSave = result.ToPricePocoList();

        await context.AddRangeAsync(pricesToSave);
        await context.SaveChangesAsync();
        Log.Information(
            "Sucessfully saved to {Count} prices entries from API call for prices",
            pricesToSave.Count
        );
    }

    private static async Task FillTable<T>() where T : class
    {
        using var context = GoblinDbContext;
        if (context is null)
            throw new Exception("Critical error: unable to get database context");

        var tableName = LogTaskStart<T>("Loading from CSV");

        var path = ResourceFilePath(ResourceFilenameCsv(tableName));
        try
        {
            await LoadCSVFileAndSaveResults<T>(context, tableName, path);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }

    private static async Task LoadCSVFileAndSaveResults<T>(
        GilGoblinDbContext context,
        string tableName,
        string path
    ) where T : class
    {
        var result = CsvInteractor<T>.LoadFile(path);

        if (result.Any())
        {
            await context.AddRangeAsync(result);
            await context.SaveChangesAsync();
            Log.Information(
                "Sucessfully saved to table {TableName} {ResultCount} entries from CSV",
                tableName,
                result.Count()
            );
        }
    }

    private static string LogTaskStart<T>(string sourceSuffix) where T : class
    {
        var pocoName = typeof(T).ToString().Split(".")[2];
        var tableName = pocoName.Remove(pocoName.Length - 4);
        Log.Warning("Database table {TableName} has missing entries. " + sourceSuffix, tableName);
        return tableName;
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

    public void Disconnect()
    {
        Connection?.Close();
        Connection?.Dispose();
    }

    public static async Task Save()
    {
        try
        {
            using var context = GoblinDbContext;
            Log.Debug("Saving to database.");
            var savedEntries = await context.SaveChangesAsync();
            Log.Debug("Saved {saved} entries to the database.", savedEntries);
        }
        catch (Exception ex)
        {
            Log.Error("Database save failed! {Message}.", ex.Message);
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
    private const int TestWorldID = 34;
    public static readonly int ApiSpamPreventionDelayInMS = 100;
}
