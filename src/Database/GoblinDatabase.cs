using Serilog;
using GilGoblin.Services;
using GilGoblin.Pocos;
using GilGoblin.Web;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Net.Http;
using System.Collections.Generic;
using GilGoblin.Extensions;

namespace GilGoblin.Database;

public class GoblinDatabase
{
    private readonly IPriceDataFetcher _priceFetcher;

    private static GilGoblinDbContext _dbContext;

    public GoblinDatabase(IPriceDataFetcher priceFetcher)
    {
        _priceFetcher = priceFetcher;
    }

    public async Task<GilGoblinDbContext?> GetContextAsync() => _dbContext ?? await GetNewContext();

    private async Task<GilGoblinDbContext> GetNewContext()
    {
        GilGoblinDatabaseConnector.Connect();
        if (GilGoblinDatabaseConnector.Connection is not { })
            return null;

        await FillTablesIfEmpty();
        _dbContext = new GilGoblinDbContext();
        return _dbContext;
    }

    private async Task FillTablesIfEmpty()
    {
        try
        {
            using var context = _dbContext;
            if (context is null || context.ItemInfo is null || context.Recipe is null)
                return;
            await context.Database.EnsureCreatedAsync();

            if (context.ItemInfo.Count() < 1000)
                await FillTable<ItemInfoPoco>();

            if (context.Recipe.Count() < 1000)
                await FillTable<RecipePoco>();

            // if (context.Price?.Count() < 1000)
            //     await FillTable<PricePoco>();

            if (context.Price?.Count() < 1000)
                await FetchPrices();
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
        }
    }

    private async Task FetchPrices()
    {
        LogTaskStart<PriceWebPoco>("Fetching prices from API");

        var batches = await _priceFetcher.GetAllIDsAsBatchJobsAsync(testWorldID);

        foreach (var batch in batches)
        {
            var timer = new Stopwatch();
            timer.Start();
            var result = await _priceFetcher.FetchMultiplePricesAsync(testWorldID, batch);

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
        using var context = _dbContext;

        var pricesToSave = result.ToPricePocoList();

        await context.AddRangeAsync(pricesToSave);
        await context.SaveChangesAsync();
        Log.Information(
            "Sucessfully saved to {Count} prices entries from API call for prices",
            pricesToSave.Count
        );
    }

    private static async Task FillTable<T>()
        where T : class
    {
        using var context =
            _dbContext ?? throw new Exception("Critical error: unable to get database context");
        var tableName = LogTaskStart<T>("Loading from CSV");

        var path = GilGoblinDatabaseConnector.ResourceFilePath(
            GilGoblinDatabaseConnector.ResourceFilenameCsv(tableName)
        );
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
    )
        where T : class
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

    private static string LogTaskStart<T>(string sourceSuffix)
        where T : class
    {
        var pocoName = typeof(T).ToString().Split(".")[2];
        var tableName = pocoName.Remove(pocoName.Length - 4);
        Log.Warning("Database table {TableName} has missing entries. " + sourceSuffix, tableName);
        return tableName;
    }

    public static async Task Save()
    {
        try
        {
            using var context = _dbContext;
            Log.Debug("Saving to database.");
            var savedEntries = await context.SaveChangesAsync();
            Log.Debug("Saved {saved} entries to the database.", savedEntries);
        }
        catch (Exception ex)
        {
            Log.Error("Database save failed! {Message}.", ex.Message);
        }
    }

    private const int testWorldID = 34;
    public static readonly int ApiSpamPreventionDelayInMS = 100;
}
