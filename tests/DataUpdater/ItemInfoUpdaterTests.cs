// using GilGoblin.Database;
// using GilGoblin.DataUpdater;
// using GilGoblin.Exceptions;
// using GilGoblin.Pocos;
// using GilGoblin.Web;
// using Microsoft.Extensions.Logging;
// using NSubstitute;
// using NSubstitute.ExceptionExtensions;
// using NUnit.Framework;
//
// namespace GilGoblin.Tests.DataUpdater;
//
// public class ItemInfoUpdaterTests : InMemoryTestDb
// {
//     private ItemInfoUpdater _updater;
//     private GilGoblinDbContext _dbContext;
//     private IItemInfoFetcher _fetcher;
//     private IDataSaver<ItemInfoWebPoco> _saver;
//     private ILogger<ItemInfoUpdater> _logger;
//     private string _urlToUse;
//
//     [Test]
//     public async Task GivenUpdateAsync_WhenThereAreNoEntriesToUpdate_ThenWeDoNotMakeAnyCalls()
//     {
//         await _updater.StartAsync(CancellationToken.None);
//
//         _logger.LogInformation($"No entries need to be updated for {nameof(ItemInfoPoco)}");
//         await _fetcher.DidNotReceive().FetchByUrlAsync(Arg.Any<string>());
//     }
//
//     [Test]
//     public async Task GetMarketableItemIDsAsync_WhenAnEmptyResponseIsReceived_ThenWeReturnEmptyAndLogTheError()
//     {
//         await _updater.StartAsync(CancellationToken.None);
//
//         _logger.LogInformation($"No entries need to be updated for {nameof(ItemInfoPoco)}");
//         await _fetcher.DidNotReceive().FetchByUrlAsync(Arg.Any<string>());
//     }
//
//     [Test]
//     public async Task GivenUpdateAsync_WhenThereAreEntriesToUpdate_ThenWeCallFetchMultiple()
//     {
//         var newEntries = new List<ItemInfoPoco>()
//         {
//             new() { ID = 4433, Name = "test1" }, new() { ID = 8432, Name = "test2" }
//         };
//         var response = new ItemInfoWebResponse(newEntries);
//         var idsToUpdate = newEntries.Select(item => item.ID).ToList();
//         _fetcher.GetMarketableItemIDsAsync().Returns(idsToUpdate);
//         _fetcher.FetchByIdsAsync(Arg.Any<IEnumerable<int>>())
//             .Returns(response.Items.Values.ToList());
//         _updater = new ItemInfoUpdater(_dbContext, _fetcher, _saver, _logger);
//
//         await _updater.StartAsync(CancellationToken.None);
//
//         await _fetcher.Received(1)
//             .FetchByIdsAsync(Arg.Is<IEnumerable<int>>(i => i.Count() == idsToUpdate.Count));
//         var successMessage = $"Received updates for {idsToUpdate.Count} {nameof(ItemInfoWebPoco)} entries";
//         _logger.Received().LogInformation(successMessage);
//         _logger.DidNotReceive().LogInformation($"No entries need to be updated for {nameof(ItemInfoWebPoco)}");
//     }
//
//     [Test]
//     public async Task GivenUpdateAsync_WhenThereAreEntriesToUpdate_ThenWeSaveToTheDatabase()
//     {
//         var newEntries = new List<ItemInfoPoco>() { new() { ID = 57745, Name = "saveMe" } };
//         var response = new ItemInfoWebResponse(newEntries);
//         var idsToUpdate = newEntries.Select(item => item.ID).ToList();
//         _fetcher.GetMarketableItemIDsAsync().Returns(idsToUpdate);
//         _fetcher.FetchByIdsAsync(Arg.Any<IEnumerable<int>>())
//             .Returns(response.Items.Values.ToList());
//         _updater = new ItemInfoUpdater(_dbContext, _fetcher, _saver, _logger);
//
//         await _updater.StartAsync(CancellationToken.None);
//
//         var checkIt = new GilGoblinDbContext(_options, _configuration);
//         var exists = checkIt.ItemInfo.FirstOrDefault(i => i.ID == 57745);
//         Assert.That(exists, Is.Not.Null);
//         // _logger.Received()
//         //     .LogInformation($"Saved {idsToUpdate.Count()} entries for type {nameof(ItemInfoWebPoco)}");
//     }
//
//     [Test]
//     public async Task GivenUpdateAsync_WhenThereAreEntriesToUpdateAndFetchingThrows_ThenWeDoNotCallAndLogTheError()
//     {
//         _fetcher.GetMarketableItemIDsAsync().Returns(new List<int> { 90010, 90011, 90012 });
//         _fetcher.FetchByIdsAsync(Arg.Any<IEnumerable<int>>()).Throws<DatabaseException>();
//         _updater = new ItemInfoUpdater(_dbContext, _fetcher, _saver, _logger);
//
//         await _updater.StartAsync(CancellationToken.None);
//
//         await _fetcher.DidNotReceive().FetchByUrlAsync(Arg.Any<string>());
//         _logger.Received()
//             .LogError($"Failed to fetch updates for {nameof(ItemInfoWebPoco)}: Error in the application.");
//     }
//
//     [OneTimeSetUp]
//     public override void OneTimeSetUp()
//     {
//         _urlToUse = "www.baseurl.com";
//         base.OneTimeSetUp();
//     }
//
//     [SetUp]
//     public override void SetUp()
//     {
//         base.SetUp();
//         _dbContext = new GilGoblinDbContext(_options, _configuration);
//         _fetcher = Substitute.For<IItemInfoFetcher>();
//         _logger = Substitute.For<ILogger<ItemInfoUpdater>>();
//         _saver = Substitute.For<IDataSaver<ItemInfoWebPoco>>();
//
//         _updater = new ItemInfoUpdater(_dbContext, _fetcher, _saver, _logger);
//     }
// }

