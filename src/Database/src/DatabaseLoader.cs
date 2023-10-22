// using System.Linq;
// using System;
// using System.Threading.Tasks;
// using GilGoblin.Database.Pocos;
// using Microsoft.Extensions.Logging;
//
// namespace GilGoblin.Database;
//
// public interface IDatabaseLoader
// {
//     Task FillTablesIfEmpty();
// }
//
// public class DatabaseLoader : IDatabaseLoader
// {
//     private readonly GilGoblinDbContext _dbContext;
//     private readonly ICsvInteractor _csvInteractor;
//     private readonly ILogger<DatabaseLoader> _logger;
//
//     public DatabaseLoader(
//         GilGoblinDbContext dbContext,
//         ICsvInteractor csvInteractor,
//         ILogger<DatabaseLoader> logger)
//     {
//         _csvInteractor = csvInteractor;
//         _logger = logger;
//         _dbContext = dbContext;
//     }
//
//     public async Task FillTablesIfEmpty()
//     {
//         try
//         {
//             await using var context = _dbContext;
//             await context.Database.EnsureDeletedAsync();
//             var existsAsync = await context.Database.EnsureCreatedAsync();
//             if (!existsAsync)
//                 throw new Exception("Database does not exist and could not be created");
//
//             // if (context.ItemInfo.Count() < 1000)
//             //     await FillTable<ItemInfoPoco>();
//
//             // if (context.Recipe.Count() < 1000)
//             //     await FillTable<RecipePoco>(dbContext);
//             //
//             // if (context.Price.Count() < 1000)
//             //     await FillTable<PricePoco>(dbContext);
//
//             await context.SaveChangesAsync();
//         }
//         catch (Exception e)
//         {
//             _logger.LogError($"Failed to initialize database: {e.Message}");
//         }
//     }
//
//     private async Task FillTable<T>() where T : class
//     {
//         var pocoName = typeof(T).ToString().Split(".").Last();
//         var tableName = pocoName.Remove(pocoName.Length - 4);
//         var message = $"Database table {tableName} has missing entries. Populating with entries from csv file";
//         _logger.LogWarning(message);
//         await LoadCsvFileAndSaveResults<T>(tableName);
//     }
//
//     private async Task LoadCsvFileAndSaveResults<T>(string tableName) where T : class
//     {
//         try
//         {
//             var result = _csvInteractor.LoadFile<T>(tableName);
//             if (!result.Any())
//                 throw new Exception($"No entries loaded for file of {nameof(T)}");
//
//             await using var dbContext = _dbContext;
//             await dbContext.AddRangeAsync(result);
//             await dbContext.SaveChangesAsync();
//             _logger.LogInformation(
//                 $"Successfully saved to table {tableName} {result.Count} entries from CSV"
//             );
//         }
//         catch (Exception e)
//         {
//             _logger.LogError($"Failed to load CSV file: {e.Message}");
//         }
//     }
//
//     public static readonly int TestWorldId = 34;
//     public static readonly int ApiSpamPreventionDelayInMS = 100;
// }