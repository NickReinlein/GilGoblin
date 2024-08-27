// using System;
// using System.Collections.Generic;
// using System.Linq;
// using System.Threading;
// using System.Threading.Tasks;
// using GilGoblin.Api.Repository;
// using GilGoblin.Database;
// using GilGoblin.Database.Pocos;
// using GilGoblin.Database.Savers;
// using GilGoblin.DataUpdater;
// using GilGoblin.Fetcher;
// using GilGoblin.Fetcher.Pocos;
// using GilGoblin.Tests.InMemoryTest;
// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.Extensions.Logging;
// using NSubstitute;
// using NSubstitute.ClearExtensions;
// using NSubstitute.ExceptionExtensions;
// using NUnit.Framework;
//
// namespace GilGoblin.Tests.DataUpdater;
//
// public class PriceUpdaterTests : InMemoryTestDb
// {
//     private IMarketableItemIdsFetcher _marketableIdsFetcher;
//     private IPriceFetcher _priceFetcher;
//     private PriceUpdater _priceUpdater;
//     private IDataSaver<PricePoco> _saver;
//     private IPriceRepository<PricePoco> _priceRepo;
//     private ILogger<PriceUpdater> _logger;
//     private IRecipeRepository _recipeRepo;
//
//     private IServiceScopeFactory _scopeFactory;
//     private IServiceScope _scope;
//     private IServiceProvider _serviceProvider;
//     private TestGilGoblinDbContext _dbContext;
//
//     private const int worldId = 34;
//     private const int regionId = 28;
//     private const int itemId1 = 1500;
//     private const int itemId2 = 3614;
//
//     [SetUp]
//     public override void SetUp()
//     {
//         base.SetUp();
//         _dbContext = new TestGilGoblinDbContext(_options, _configuration);
//         _marketableIdsFetcher = Substitute.For<IMarketableItemIdsFetcher>();
//         _scopeFactory = Substitute.For<IServiceScopeFactory>();
//         _priceFetcher = Substitute.For<IPriceFetcher>();
//         _priceRepo = Substitute.For<IPriceRepository<PricePoco>>();
//         _recipeRepo = Substitute.For<IRecipeRepository>();
//         _saver = Substitute.For<IDataSaver<PricePoco>>();
//         _logger = Substitute.For<ILogger<PriceUpdater>>();
//         _scope = Substitute.For<IServiceScope>();
//         _serviceProvider = Substitute.For<IServiceProvider>();
//
//         _scopeFactory.CreateScope().Returns(_scope);
//         _scope.ServiceProvider.Returns(_serviceProvider);
//         _serviceProvider.GetService(typeof(IMarketableItemIdsFetcher)).Returns(_marketableIdsFetcher);
//         _serviceProvider.GetService(typeof(GilGoblinDbContext)).Returns(_dbContext);
//         _serviceProvider.GetService(typeof(IPriceFetcher)).Returns(_priceFetcher);
//         _serviceProvider.GetService(typeof(IDataSaver<PricePoco>)).Returns(_saver);
//         _serviceProvider.GetService(typeof(IPriceRepository<PricePoco>)).Returns(_priceRepo);
//         _serviceProvider.GetService(typeof(IRecipeRepository)).Returns(_recipeRepo);
//
//         var prices = _dbContext.Price.Where(p => p.WorldId == 34).ToList();
//         _priceRepo.GetAll(34).Returns(prices);
//
//         var recipes = _dbContext.Recipe.ToList();
//         _recipeRepo.GetAll().Returns(recipes);
//
//         _priceUpdater = new PriceUpdater(_scopeFactory, _logger);
//     }
//
//     [Test]
//     public async Task GivenFetchAsync_WhenNoMarketableItemsAreReturned_ThenWeLogAnError()
//     {
//         const string errorMessage = "Failed to get the Ids to update for world 34: Failed to fetch marketable item ids";
//         _marketableIdsFetcher.GetMarketableItemIdsAsync().Returns(new List<int>());
//
//         var cts = new CancellationTokenSource();
//         cts.CancelAfter(500);
//         await _priceUpdater.FetchAsync(cts.Token, 34);
//
//         await _marketableIdsFetcher.Received(1).GetMarketableItemIdsAsync();
//         await _priceFetcher.DidNotReceiveWithAnyArgs().FetchByIdsAsync(default, default!);
//         _logger.Received(1).LogError(errorMessage);
//     }
//
//     [TestCase(0)]
//     [TestCase(-1)]
//     [TestCase(null)]
//     public async Task GivenFetchAsync_WhenTheWorldIdIsInvalid_ThenWeLogAnError(int? worldIdString)
//     {
//         var cts = new CancellationTokenSource();
//         cts.CancelAfter(500);
//         await _priceUpdater.FetchAsync(cts.Token, worldIdString);
//
//         var message = $"Failed to get the Ids to update for world {worldIdString}: World Id is invalid";
//         _logger.Received().LogError(message);
//     }
//
//     [Test]
//     public void GivenFetchAsync_WhenTheTokenIsCancelled_ThenWeExitGracefully()
//     {
//         _marketableIdsFetcher.GetMarketableItemIdsAsync().Returns([443, 1420, 3500, 900]);
//         _saver.SaveAsync(default!).ReturnsForAnyArgs(true);
//         _priceFetcher.GetEntriesPerPage().Returns(2);
//         _priceFetcher.FetchByIdsAsync(Arg.Any<CancellationToken>(), Arg.Any<IEnumerable<int>>(), Arg.Any<int?>())
//             .Returns(new List<PriceWebPoco>());
//
//         var cts = new CancellationTokenSource();
//         cts.CancelAfter(500);
//         Assert.DoesNotThrowAsync(async () => await _priceUpdater.FetchAsync(cts.Token, 34));
//
//         _logger.Received().LogInformation($"Awaiting delay of {5000}ms before next batch call (Spam prevention)");
//     }
//
//     [Test]
//     public async Task GivenConvertAndSaveToDbAsync_WhenThereAreValidEntriesToSave_ThenWeConvertAndSaveThoseEntities()
//     {
//         var saveList = SetupSave();
//
//         var cts = new CancellationTokenSource();
//         cts.CancelAfter(500);
//         await _priceUpdater.FetchAsync(cts.Token, 34);
//
//         Assert.That(saveList, Has.Count.GreaterThanOrEqualTo(2));
//         await _saver.Received().SaveAsync(
//             Arg.Is<List<PricePoco>>(x => saveList.All(
//                 s => x.Any(i =>
//                     i.ItemId == s.ItemId &&
//                     i.WorldId == s.WorldId)))
//         );
//     }
//
//     [Test]
//     public async Task GivenConvertAndSaveToDbAsync_WhenSavingThrowsAnException_ThenWeLogTheError()
//     {
//         var saveList = SetupSave();
//         _saver.ClearSubstitute();
//         _saver.SaveAsync(default!).ThrowsAsyncForAnyArgs(new Exception("test"));
//
//         var cts = new CancellationTokenSource();
//         cts.CancelAfter(500);
//         await _priceUpdater.FetchAsync(cts.Token, 34);
//
//         _logger.Received().LogError($"Failed to save {saveList.Count} entries for {nameof(PricePoco)}: test");
//     }
//
//     [Test]
//     public async Task GivenConvertAndSaveToDbAsync_WhenSavingReturnsFalse_ThenWeLogTheError()
//     {
//         var saveList = SetupSave();
//         _saver.SaveAsync(Arg.Any<IEnumerable<PricePoco>>()).Returns(false);
//         var errorMessage =
//             $"Failed to save {saveList.Count} entries for {nameof(PricePoco)}: Saving from {nameof(IDataSaver<PricePoco>)} returned failure";
//
//         var cts = new CancellationTokenSource();
//         cts.CancelAfter(500);
//         await _priceUpdater.FetchAsync(cts.Token, 34);
//
//         _logger.Received().LogError(errorMessage);
//     }
//
//
//     private List<PricePoco> SetupSave()
//     {
//         var saveList = GetMultipleNewPocos();
//
//         var idList = new List<int> { 443, 1420 };
//         _marketableIdsFetcher.GetMarketableItemIdsAsync().Returns(idList);
//         _saver.SaveAsync(default!).ReturnsForAnyArgs(true);
//         _priceFetcher.GetEntriesPerPage().Returns(2);
//         _priceFetcher
//             .FetchByIdsAsync(Arg.Any<CancellationToken>(), Arg.Any<IEnumerable<int>>(), Arg.Any<int?>())
//             .Returns(saveList);
//         return saveList;
//     }
//     
//     protected static List<PriceWebPoco> GetMultipleNewPocos()
//     {
//         var priceGeoDataPointsPoco = new PriceDataPerGeoPoco(
//             new PriceDataPoco(900),
//             new PriceDataPoco(800, worldId),
//             new PriceDataPoco(700, regionId)
//         );
//         var anyQ = new GeoPriceDataPoco(
//             priceGeoDataPointsPoco,
//             priceGeoDataPointsPoco,
//             priceGeoDataPointsPoco,
//             new DailySaleVelocityPoco(50)
//         );
//         var worldUploadTimestampPocos = new List<WorldUploadTimestampPoco>
//         {
//             new(worldId, 67554), new(worldId + 1, 67555)
//         };
//
//         var poco1 = new PriceWebPoco(
//             itemId1,
//             anyQ,
//             anyQ,
//             worldUploadTimestampPocos
//         );
//         var poco2 = new PriceWebPoco(
//             itemId2,
//             anyQ,
//             anyQ,
//             worldUploadTimestampPocos
//         );
//
//         return new List<PriceWebPoco> { poco1, poco2 };
//     }
// }