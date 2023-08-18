using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Pocos;
using GilGoblin.Services;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database;

public class GilGoblinDatabaseInitializer
{
    private readonly ISqlLiteDatabaseConnector _dbConnector;
    private readonly ICsvInteractor _csvInteractor;
    private readonly ILogger<GilGoblinDatabaseInitializer> _logger;

    public GilGoblinDatabaseInitializer(
        ISqlLiteDatabaseConnector dbConnector,
        ICsvInteractor csvInteractor,
        ILogger<GilGoblinDatabaseInitializer> logger
    )
    {
        _dbConnector = dbConnector;
        _csvInteractor = csvInteractor;
        _logger = logger;
    }

    public async Task FillTablesIfEmpty(GilGoblinDbContext dbContext)
    {
        try
        {
            using var context = dbContext;
            await context.Database.EnsureCreatedAsync();

            if (context.ItemInfo.Count() < 1000)
                await FillTable<ItemInfoPoco>(dbContext);

            if (context.Recipe.Count() < 1000)
                await FillTable<RecipePoco>(dbContext);

            if (context.Price?.Count() < 1000)
                await FillTable<PricePoco>(dbContext);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
        }
    }

    // private async Task FetchPrices(GilGoblinDbContext dbContext)
    // {
    //     LogTaskStart<PriceWebPoco>("Fetching prices from API");
    //     var batches = await _priceFetcher.GetAllIDsAsBatchJobsAsync(TestWorldID);

    //     foreach (var batch in batches)
    //     {
    //         var timer = new Stopwatch();
    //         timer.Start();
    //         var result = await _priceFetcher.FetchMultiplePricesAsync(TestWorldID, batch);

    //         if (!result.Any())
    //             throw new HttpRequestException("Failed to fetch prices from Universalis API");

    //         await SaveBatchResult(dbContext, result);

    //         timer.Stop();
    //         Log.Information("Total time for batch in ms: {Elapsed}", timer.ElapsedMilliseconds);
    //         await Task.Delay(ApiSpamPreventionDelayInMS);
    //     }
    // }

    public async Task SaveBatchResult(
        GilGoblinDbContext dbContext,
        IEnumerable<PriceWebPoco?> batchTosSave
    )
    {
        var pricesToSave = batchTosSave.ToPricePocoList();
        if (!pricesToSave.Any())
            return;

        using var context = dbContext;
        await context.AddRangeAsync(pricesToSave);
        await context.SaveChangesAsync();
        _logger.LogInformation(
            $"Successfully saved to {pricesToSave.Count} prices to the database"
        );
    }

    private async Task FillTable<T>(GilGoblinDbContext dbContext)
        where T : class
    {
        var tableName = LogTaskStart<T>("Loading from CSV");
        var path = _dbConnector.GetDatabasePath();
        await LoadCSVFileAndSaveResults<T>(dbContext, tableName, path);
    }

    private async Task LoadCSVFileAndSaveResults<T>(
        GilGoblinDbContext context,
        string tableName,
        string path
    )
        where T : class
    {
        var result = _csvInteractor.LoadFile<T>(path);

        if (result.Any())
        {
            await context.AddRangeAsync(result);
            await context.SaveChangesAsync();
            _logger.LogInformation(
                $"Sucessfully saved to table {tableName} {result.Count} entries from CSV"
            );
        }
    }

    private string LogTaskStart<T>(string sourceSuffix)
        where T : class
    {
        var pocoName = typeof(T).ToString().Split(".")[2];
        var tableName = pocoName.Remove(pocoName.Length - 4);
        _logger.LogWarning($"Database table {tableName} has missing entries {sourceSuffix}");
        return tableName;
    }

    public static readonly int TestWorldID = 34;
    public static readonly int ApiSpamPreventionDelayInMS = 100;
}
