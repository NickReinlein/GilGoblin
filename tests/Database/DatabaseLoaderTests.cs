// using GilGoblin.Pocos;
// using GilGoblin.Services;
// using GilGoblin.Database;
// using GilGoblin.Database.Pocos;
// using NSubstitute;
// using NUnit.Framework;
// using Microsoft.Extensions.Logging;
// using NSubstitute.ExceptionExtensions;
//
// namespace GilGoblin.Tests.Database;
//
// public class DatabaseLoaderTests : InMemoryTestDb
// {
//     private ICsvInteractor _csvInteractor;
//     private ILogger<DatabaseLoader> _logger;
//
//     private DatabaseLoader _databaseLoader;
//     private GilGoblinDbContext _dbContext;
//
//     [Test]
//     public async Task GiveWeCallFillTablesIfEmpty_WhenTableItemInfoIsEmpty_ThenWeFillTheTable()
//     {
//         await using var context = new GilGoblinDbContext(_options, _configuration);
//
//         await _databaseLoader.FillTablesIfEmpty();
//
//         await using var context2 = new GilGoblinDbContext(_options, _configuration);
//         Assert.That(context2.ItemInfo.Count, Is.GreaterThan(0));
//     }
//
//     [Test]
//     public async Task GiveWeCallFillTablesIfEmpty_WhenTablePriceIsEmpty_ThenWeFillTheTable()
//     {
//         await using var context = new GilGoblinDbContext(_options, _configuration);
//
//         await _databaseLoader.FillTablesIfEmpty();
//
//         await using var context2 = new GilGoblinDbContext(_options, _configuration);
//         Assert.That(context2.Price.Count, Is.GreaterThan(0));
//     }
//
//     [Test]
//     public async Task GiveWeCallFillTablesIfEmpty_WhenTableRecipeIsEmpty_ThenWeFillTheTable()
//     {
//         await using var context = new GilGoblinDbContext(_options, _configuration);
//
//         await _databaseLoader.FillTablesIfEmpty();
//
//         await using var context2 = new GilGoblinDbContext(_options, _configuration);
//         Assert.That(context2.Recipe.Count, Is.GreaterThan(0));
//     }
//
//     [Test]
//     public void GiveWeHaveADatabaseInitializer_WhenWeGetGlobalParameters_ThenTheyAreSet()
//     {
//         Assert.Multiple(() =>
//         {
//             Assert.That(
//                 DatabaseLoader.ApiSpamPreventionDelayInMS,
//                 Is.GreaterThan(10)
//             );
//             Assert.That(DatabaseLoader.TestWorldId, Is.GreaterThan(0));
//         });
//     }
//
//     [Test]
//     public async Task GiveWeCallFillTablesIfEmpty_WhenAnExceptionIsThrown_ThenWeLogAnError()
//     {
//         _csvInteractor.LoadFile<ItemInfoPoco>("TestDatabase").Throws<Exception>();
//         await using var context = new GilGoblinDbContext(_options, _configuration);
//
//         await _databaseLoader.FillTablesIfEmpty();
//
//         _logger.ReceivedWithAnyArgs(1).LogError(default);
//     }
//
//     [SetUp]
//     public override void SetUp()
//     {
//         base.SetUp();
//         _logger = Substitute.For<ILogger<DatabaseLoader>>();
//         _dbContext = new GilGoblinDbContext(_options, _configuration);
//         SetupCsvInteractor();
//
//         _databaseLoader = new DatabaseLoader(
//             _dbContext,
//             _csvInteractor,
//             _logger
//         );
//     }
//
//     private static List<PriceWebPoco> GenerateListOf2ValidPocos()
//     {
//         return new List<PriceWebPoco?>
//         {
//             new()
//             {
//                 ItemId = 456,
//                 WorldId = 23,
//                 LastUploadTime = 1677798249999,
//                 AveragePrice = 512,
//                 CurrentAveragePrice = 400,
//             },
//             new()
//             {
//                 ItemId = 789,
//                 WorldId = 23,
//                 LastUploadTime = 1677798949999,
//                 AveragePrice = 1024,
//                 CurrentAveragePrice = 800,
//             },
//         };
//     }
//
//     private void SetupCsvInteractor()
//     {
//         _csvInteractor = Substitute.For<ICsvInteractor>();
//         _csvInteractor
//             .LoadFile<ItemInfoPoco>("TestDatabase")
//             .Returns(
//                 new List<ItemInfoPoco>() { new() { Id = 123 }, new() { Id = 456 } }
//             );
//         _csvInteractor
//             .LoadFile<RecipePoco>("TestDatabase")
//             .Returns(
//                 new List<RecipePoco>()
//                 {
//                     new() { Id = 123, TargetItemId = 33, ResultQuantity = 1 },
//                     new() { Id = 456, TargetItemId = 44, ResultQuantity = 1 }
//                 }
//             );
//         _csvInteractor
//             .LoadFile<PricePoco>("TestDatabase")
//             .Returns(
//                 new List<PricePoco>()
//                 {
//                     new()
//                     {
//                         ItemId = 456,
//                         WorldId = 23,
//                         LastUploadTime = 1677798249999,
//                         AverageListingPrice = 512,
//                         AverageSold = 400,
//                     },
//                     new()
//                     {
//                         ItemId = 123,
//                         WorldId = 23,
//                         LastUploadTime = 1677798249750,
//                         AverageListingPrice = 256,
//                         AverageSold = 200,
//                     }
//                 }
//             );
//     }
// }