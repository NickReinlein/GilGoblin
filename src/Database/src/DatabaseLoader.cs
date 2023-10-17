using System.Linq;
using System;
using System.Data;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Services;
using Microsoft.Extensions.Logging;

namespace GilGoblin.Database;

public interface IDatabaseLoader
{
    Task FillTablesIfEmpty();
}

public class DatabaseLoader : IDatabaseLoader
{
    private readonly GilGoblinDbContext _dbContext;
    private readonly ISqlLiteDatabaseConnector _dbConnector;
    private readonly ICsvInteractor _csvInteractor;
    private readonly ILogger<DatabaseLoader> _logger;

    public DatabaseLoader(
        GilGoblinDbContext dbContext,
        ISqlLiteDatabaseConnector dbConnector,
        ICsvInteractor csvInteractor,
        ILogger<DatabaseLoader> logger)
    {
        _dbConnector = dbConnector;
        _csvInteractor = csvInteractor;
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task FillTablesIfEmpty()
    {
        try
        {
            await using var context = _dbContext;
            var sqliteConnection = _dbConnector?.Connect();
            if (sqliteConnection?.State != ConnectionState.Open)
                throw new Exception("Unable to establish a connection to the database");

            await context.Database.EnsureCreatedAsync();

            if (context.ItemInfo.Count() < 1000)
                await FillTable<ItemInfoPoco>(_dbContext);

            // if (context.Recipe.Count() < 1000)
            //     await FillTable<RecipePoco>(dbContext);
            //
            // if (context.Price.Count() < 1000)
            //     await FillTable<PricePoco>(dbContext);
            // await context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError($"Failed to initialize database: {e.Message}");
        }
    }

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