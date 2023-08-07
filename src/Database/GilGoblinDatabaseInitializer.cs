// using System;
// using System.Collections.Generic;
// using System.Diagnostics;
// using System.Linq;
// using System.Net.Http;
// using System.Threading.Tasks;
// using GilGoblin.Pocos;
// using GilGoblin.Services;
// using GilGoblin.Web;
// using GilGoblin.Extensions;
// using Serilog;
// using System.Data.Entity;

// namespace GilGoblin.Database;

// public class GilGoblinDatabaseInitializer
// {
//     private readonly IPriceDataFetcher _priceFetcher;
//     private readonly ISqlLiteDatabaseConnector _dbConnector;

//     public GilGoblinDatabaseInitializer(
//         IPriceDataFetcher priceFetcher,
//         ISqlLiteDatabaseConnector dbConnector
//     )
//     {
//         _priceFetcher = priceFetcher;
//         _dbConnector = dbConnector;
//     }

//     public async Task FillTablesIfEmpty(GilGoblinDbContext dbContext)
//     {
//         try
//         {
//             using var context = dbContext;
//             if (context is null || context.ItemInfo is null || context.Recipe is null)
//                 return;
//             await context.Database.EnsureCreatedAsync();

//             if (context.ItemInfo.Count() < 1000)
//                 await FillTable<ItemInfoPoco>(dbContext);

//             if (context.Recipe.Count() < 1000)
//                 await FillTable<RecipePoco>(dbContext);

//             // if (context.Price?.Count() < 1000)
//             //     await FillTable<PricePoco>();

//             if (context.Price?.Count() < 1000)
//                 await FetchPrices(dbContext);
//         }
//         catch (Exception e)
//         {
//             Log.Error(e.Message);
//         }
//     }

//     private async Task FetchPrices(GilGoblinDbContext dbContext)
//     {
//         LogTaskStart<PriceWebPoco>("Fetching prices from API");

//         var batches = await _priceFetcher.GetAllIDsAsBatchJobsAsync(TestWorldID);

//         foreach (var batch in batches)
//         {
//             var timer = new Stopwatch();
//             timer.Start();
//             var result = await _priceFetcher.FetchMultiplePricesAsync(TestWorldID, batch);

//             if (!result.Any())
//                 throw new HttpRequestException("Failed to fetch prices from Universalis API");

//             await SaveBatchResult(dbContext, result);

//             timer.Stop();
//             Log.Information("Total time for batch in ms: {Elapsed}", timer.ElapsedMilliseconds);
//             await Task.Delay(ApiSpamPreventionDelayInMS);
//         }
//     }

//     public async Task SaveBatchResult(
//         GilGoblinDbContext dbContext,
//         IEnumerable<PriceWebPoco?> result
//     )
//     {
//         using var context = dbContext;

//         var pricesToSave = result.ToPricePocoList();

//         await context.AddRangeAsync(pricesToSave);
//         await context.SaveChangesAsync();
//         Log.Information(
//             "Sucessfully saved to {Count} prices entries from API call for prices",
//             pricesToSave.Count
//         );
//     }

//     private async Task FillTable<T>(GilGoblinDbContext dbContext)
//         where T : class
//     {
//         var tableName = LogTaskStart<T>("Loading from CSV");

//         // fix me

//         var path = _dbConnector.GetDatabasePath();
//         try
//         {
//             await LoadCSVFileAndSaveResults<T>(dbContext, tableName, path);
//         }
//         catch (Exception e)
//         {
//             Log.Error(e.Message);
//         }
//     }

//     private static async Task LoadCSVFileAndSaveResults<T>(
//         GilGoblinDbContext context,
//         string tableName,
//         string path
//     )
//         where T : class
//     {
//         var result = CsvInteractor<T>.LoadFile(path);

//         if (result.Any())
//         {
//             await context.AddRangeAsync(result);
//             await context.SaveChangesAsync();
//             Log.Information(
//                 "Sucessfully saved to table {TableName} {ResultCount} entries from CSV",
//                 tableName,
//                 result.Count()
//             );
//         }
//     }

//     private static string LogTaskStart<T>(string sourceSuffix)
//         where T : class
//     {
//         var pocoName = typeof(T).ToString().Split(".")[2];
//         var tableName = pocoName.Remove(pocoName.Length - 4);
//         Log.Warning("Database table {TableName} has missing entries. " + sourceSuffix, tableName);
//         return tableName;
//     }

//     public static readonly int TestWorldID = 34;
//     public static readonly int ApiSpamPreventionDelayInMS = 100;
// }
