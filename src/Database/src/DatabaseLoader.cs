using System.Linq;
using System;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Services;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database;

public interface IDatabaseLoader
{
    Task FillTablesIfEmpty(GilGoblinDbContext dbContext);
}

public class DatabaseLoader : IDatabaseLoader
{
    private readonly ISqlLiteDatabaseConnector _dbConnector;
    private readonly ICsvInteractor _csvInteractor;
    private readonly ILogger<DatabaseLoader> _logger;

    public DatabaseLoader(
        ISqlLiteDatabaseConnector dbConnector,
        ICsvInteractor csvInteractor,
        ILogger<DatabaseLoader> logger
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
            await using var context = dbContext;
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

    // public async Task SaveBatchResult(
    //     GilGoblinDbContext dbContext,
    //     IEnumerable<PriceWebPoco?> batchTosSave
    // )
    // {
    //     var pricesToSave = batchTosSave.ToPricePocoList();
    //     if (!pricesToSave.Any())
    //         return;
    //
    //     using var context = dbContext;
    //     await context.AddRangeAsync(pricesToSave);
    //     await context.SaveChangesAsync();
    //     _logger.LogInformation(
    //         $"Successfully saved to {pricesToSave.Count} prices to the database"
    //     );
    // }

    private async Task FillTable<T>(GilGoblinDbContext dbContext)
        where T : class
    {
        var tableName = LogTaskStart<T>("Loading from CSV");
        var csvPath = _dbConnector.GetResourcesPath();
        await LoadCsvFileAndSaveResults<T>(dbContext, tableName, csvPath);
    }

    private async Task LoadCsvFileAndSaveResults<T>(
        GilGoblinDbContext context,
        string tableName,
        string path
    )
        where T : class
    {
        try
        {
            var result = _csvInteractor.LoadFile<T>(path);

            if (!result.Any())
                throw new Exception($"No entries loaded for file of {nameof(T)}");

            await context.AddRangeAsync(result);
            await context.SaveChangesAsync();
            _logger.LogInformation(
                $"Successfully saved to table {tableName} {result.Count} entries from CSV"
            );
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to load CSV file: {e.Message}");
        }
    }


    private string LogTaskStart<T>(string sourceSuffix)
        where T : class
    {
        var pocoName = typeof(T).ToString().Split(".").Last();
        var tableName = pocoName.Remove(pocoName.Length - 4);
        _logger.LogWarning($"Database table {tableName} has missing entries {sourceSuffix}");
        return tableName;
    }

    public static readonly int TestWorldId = 34;
    public static readonly int ApiSpamPreventionDelayInMS = 100;
}