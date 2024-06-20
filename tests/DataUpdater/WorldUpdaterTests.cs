using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.DataUpdater;
using GilGoblin.Fetcher;
using GilGoblin.Fetcher.Pocos;
using GilGoblin.Tests.InMemoryTest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.DataUpdater;

public class WorldUpdaterTests : InMemoryTestDb
{
    private WorldUpdater _worldUpdater;
    private IWorldFetcher _fetcher;
    private IDataSaver<WorldPoco> _saver;
    private ILogger<WorldUpdater> _logger;

    private IServiceScopeFactory _scopeFactory;
    private IServiceScope _scope;
    private IServiceProvider _serviceProvider;
    private TestGilGoblinDbContext _dbContext;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _dbContext = new TestGilGoblinDbContext(_options, _configuration);
        _saver = Substitute.For<IDataSaver<WorldPoco>>();
        _fetcher = Substitute.For<IWorldFetcher>();
        _logger = Substitute.For<ILogger<WorldUpdater>>();
        _scopeFactory = Substitute.For<IServiceScopeFactory>();
        _scope = Substitute.For<IServiceScope>();
        _serviceProvider = Substitute.For<IServiceProvider>();

        _scopeFactory.CreateScope().Returns(_scope);
        _scope.ServiceProvider.Returns(_serviceProvider);
        _serviceProvider.GetService(typeof(GilGoblinDbContext)).Returns(_dbContext);
        _serviceProvider.GetService(typeof(IDataSaver<WorldPoco>)).Returns(_saver);

        _worldUpdater = new WorldUpdater(_scopeFactory, _fetcher, _logger);
    }

    [Test]
    public async Task GivenGetAllAsync_WhenNoWorldsAreReturned_ThenWeLogAnError()
    {
        _fetcher.GetAllAsync().Returns([]);

        var cts = new CancellationTokenSource();
        cts.CancelAfter(500);
        await _worldUpdater.GetAllWorldsAsync();

        await _fetcher.Received(1).GetAllAsync();
        _logger.Received(1).LogError("Received empty list returned when fetching all worlds");
    }

    [Test]
    public async Task GivenGetAllAsync_WhenWorldsAreReturned_ThenWeDoNotLogAnError()
    {
        var worlds = SetupSave();

        var cts = new CancellationTokenSource();
        cts.CancelAfter(500);
        await _worldUpdater.GetAllWorldsAsync();

        await _fetcher.Received(1).GetAllAsync();
        _logger.Received(1).LogInformation("Received updates for {Count} worlds", worlds.Count);
        _logger.DidNotReceive().LogError("Received empty list returned when fetching all worlds");
    }

    // [Test]
    // public async Task GivenFetchAsync_WhenNoMarketableItemsAreReturned_ThenWeLogAnError()
    // {
    //     const string errorMessage = "Failed to get the Ids to update for world 34: Failed to fetch marketable item ids";
    //     _marketableIdsFetcher.GetMarketableItemIdsAsync().Returns(new List<int>());
    //
    //     var cts = new CancellationTokenSource();
    //     cts.CancelAfter(500);
    //     await _worldUpdater.FetchAsync(cts.Token, 34);
    //
    //     await _marketableIdsFetcher.Received(1).GetMarketableItemIdsAsync();
    //     await _priceFetcher.DidNotReceiveWithAnyArgs().FetchByIdsAsync(default, default!);
    //     _logger.Received(1).LogError(errorMessage);
    // }
    //
    // [Test]
    // public async Task GivenFetchAsync_WhenMarketableItemsAreReturned_ThenWeAlsoReturnRecipeIngredients()
    // {
    //     var allRecipes = _dbContext.Recipe.ToList();
    //     _marketableIdsFetcher.GetMarketableItemIdsAsync().Returns([631]);
    //
    //     var cts = new CancellationTokenSource();
    //     cts.CancelAfter(500);
    //     await _worldUpdater.FetchAsync(cts.Token, 34);
    //
    //     var idCount = _worldUpdater.AllItemIds.Count;
    //     Assert.That(idCount, Is.EqualTo(allRecipes.Count + 1));
    // }
    //
    // [TestCase(0)]
    // [TestCase(-1)]
    // [TestCase(null)]
    // public async Task GivenFetchAsync_WhenTheWorldIdIsInvalid_ThenWeLogAnError(int? worldIdString)
    // {
    //     var cts = new CancellationTokenSource();
    //     cts.CancelAfter(500);
    //     await _worldUpdater.FetchAsync(cts.Token, worldIdString);
    //
    //     var message = $"Failed to get the Ids to update for world {worldIdString}: World Id is invalid";
    //     _logger.Received().LogError(message);
    // }
    //
    // [Test]
    // public void GivenFetchAsync_WhenTheTokenIsCancelled_ThenWeExitGracefully()
    // {
    //     _marketableIdsFetcher.GetMarketableItemIdsAsync().Returns([443, 1420, 3500, 900]);
    //     _saver.SaveAsync(default!).ReturnsForAnyArgs(true);
    //     _priceFetcher.GetEntriesPerPage().Returns(2);
    //     _priceFetcher.FetchByIdsAsync(Arg.Any<CancellationToken>(), Arg.Any<IEnumerable<int>>(), Arg.Any<int?>())
    //         .Returns(new List<PriceWebPoco>());
    //
    //     var cts = new CancellationTokenSource();
    //     cts.CancelAfter(500);
    //     Assert.DoesNotThrowAsync(async () => await _worldUpdater.FetchAsync(cts.Token, 34));
    //
    //     _logger.Received().LogInformation($"Awaiting delay of {5000}ms before next batch call (Spam prevention)");
    // }
    //
    // [Test]
    // public async Task GivenConvertAndSaveToDbAsync_WhenThereAreValidEntriesToSave_ThenWeConvertAndSaveThoseEntities()
    // {
    //     var saveList = SetupSave();
    //
    //     var cts = new CancellationTokenSource();
    //     cts.CancelAfter(500);
    //     await _worldUpdater.FetchAsync(cts.Token, 34);
    //
    //     Assert.That(saveList, Has.Count.GreaterThanOrEqualTo(2));
    //     await _saver.Received().SaveAsync(
    //         Arg.Is<List<PricePoco>>(x => saveList.All(
    //             s => x.Any(i =>
    //                 i.ItemId == s.ItemId &&
    //                 i.WorldId == s.WorldId)))
    //     );
    // }
    //
    // [Test]
    // public async Task GivenConvertAndSaveToDbAsync_WhenSavingThrowsAnException_ThenWeLogTheError()
    // {
    //     var saveList = SetupSave();
    //     _saver.ClearSubstitute();
    //     _saver.SaveAsync(default!).ThrowsAsyncForAnyArgs(new Exception("test"));
    //
    //     var cts = new CancellationTokenSource();
    //     cts.CancelAfter(500);
    //     await _worldUpdater.FetchAsync(cts.Token, 34);
    //
    //     _logger.Received().LogError($"Failed to save {saveList.Count} entries for {nameof(PricePoco)}: test");
    // }
    //
    // [Test]
    // public async Task GivenConvertAndSaveToDbAsync_WhenSavingReturnsFalse_ThenWeLogTheError()
    // {
    //     var saveList = SetupSave();
    //     _saver.SaveAsync(Arg.Any<IEnumerable<PricePoco>>()).Returns(false);
    //     var errorMessage =
    //         $"Failed to save {saveList.Count} entries for {nameof(PricePoco)}: Saving from {nameof(IDataSaver<PricePoco>)} returned failure";
    //
    //     var cts = new CancellationTokenSource();
    //     cts.CancelAfter(500);
    //     await _worldUpdater.FetchAsync(cts.Token, 34);
    //
    //     _logger.Received().LogError(errorMessage);
    // }

    private List<WorldPoco> SetupSave()
    {
        var saveList = new List<WorldWebPoco> { new(1, "Ramuh"), new(2, "Fenrir") };

        _saver.SaveAsync(default!).ReturnsForAnyArgs(true);
        _fetcher.GetAllAsync().Returns(saveList);

        return saveList.ToWorldPocoList();
    }
}