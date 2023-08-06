// using Serilog;
// using GilGoblin.Web;
// using System.Threading.Tasks;
// using System;
// using GilGoblin.Extensions;

// namespace GilGoblin.Database;

// public class GilGoblinDatabase
// {
//     private readonly IPriceDataFetcher _priceFetcher;
//     private readonly ISqlLiteDatabaseConnector _dbConnector;

//     public GilGoblinDatabase(IPriceDataFetcher priceFetcher, ISqlLiteDatabaseConnector dbConnector)
//     {
//         _priceFetcher = priceFetcher;
//         _dbConnector = dbConnector;
//     }

//     public GilGoblinDbContext? GetContext()
//     {
//         var connection = _dbConnector.Connect();
//         if (!connection.IsOpen())
//             return null;

//         var dbContext = new GilGoblinDbContext();
//         // var databaseInitializer = new GilGoblinDatabaseInitializer(_priceFetcher, _dbConnector);
//         // await databaseInitializer.FillTablesIfEmpty(dbContext);
//         return dbContext;
//     }
// }
