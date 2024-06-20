using System;
using System.Collections.Generic;
using System.Linq;
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
using NSubstitute.ClearExtensions;
using NSubstitute.ExceptionExtensions;
using NSubstitute.ReceivedExtensions;
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
        _logger.DidNotReceive().LogError("Received empty list returned when fetching all worlds");
        Assert.Multiple(() =>
        {
            Assert.That(worlds, Has.Count.EqualTo(2));
            Assert.That(worlds[0].Id, Is.GreaterThan(0));
            Assert.That(worlds[0].Name, Is.Not.Null.Or.Empty);
            Assert.That(worlds[1].Id, Is.GreaterThan(0));
            Assert.That(worlds[1].Name, Is.Not.Null.Or.Empty);
        });
    }

    [Test]
    public async Task GivenGetAllAsync_WhenAnExceptionIsThrown_ThenWeLogAnError()
    {
        _fetcher.GetAllAsync().ThrowsAsyncForAnyArgs(new InvalidOperationException());

        var cts = new CancellationTokenSource();
        cts.CancelAfter(500);
        await _worldUpdater.GetAllWorldsAsync();

        await _fetcher.Received(1).GetAllAsync();
        _logger.Received(1).Log(LogLevel.Error,
            "Failed to fetch updates for worlds: Operation is not valid due to the current state of the object.");
    }

    [Test, Ignore("fix me later")]
    public void GivenFetchAsync_WhenTheTokenIsCancelled_ThenWeExitGracefully()
    {
        // _marketableIdsFetcher.GetMarketableItemIdsAsync().Returns([443, 1420, 3500, 900]);
        // _saver.SaveAsync(default!).ReturnsForAnyArgs(true);
        // _priceFetcher.GetEntriesPerPage().Returns(2);
        // _priceFetcher.FetchByIdsAsync(Arg.Any<CancellationToken>(), Arg.Any<IEnumerable<int>>(), Arg.Any<int?>())
        //     .Returns(new List<PriceWebPoco>());
        //
        // var cts = new CancellationTokenSource();
        // cts.CancelAfter(500);
        // Assert.DoesNotThrowAsync(async () => await _worldUpdater.FetchAsync(cts.Token, 34));
        //
        // _logger.Received().LogInformation($"Awaiting delay of {5000}ms before next batch call (Spam prevention)");
    }

    [Test]
    public async Task GivenConvertAndSaveToDbAsync_WhenThereAreValidEntriesToSave_ThenWeConvertAndSaveThoseEntities()
    {
        var worlds = SetupSave();

        await _worldUpdater.GetAllWorldsAsync();

        Assert.That(worlds, Has.Count.GreaterThanOrEqualTo(2));
        await _saver.Received().SaveAsync(Arg.Is<List<WorldPoco>>(x =>
            x.All(y => y.Id > 0 && !string.IsNullOrWhiteSpace(y.Name))));
    }

    [Test]
    public async Task GivenConvertAndSaveToDbAsync_WhenSavingThrowsAnException_ThenWeLogTheError()
    {
        var worlds = SetupSave();
        _saver.ClearSubstitute();
        _saver.SaveAsync(default!).ThrowsAsyncForAnyArgs(new Exception("test"));

        await _worldUpdater.GetAllWorldsAsync();

        _logger.Received().LogError($"Failed to save {worlds.Count} entries for {nameof(WorldPoco)}: test");
    }

    [Test]
    public async Task GivenConvertAndSaveToDbAsync_WhenSavingReturnsFalse_ThenWeLogTheError()
    {
        var worlds = SetupSave();
        _saver.SaveAsync(Arg.Any<IEnumerable<WorldPoco>>()).Returns(false);

        await _worldUpdater.GetAllWorldsAsync();

        var errorMessage =
            $"Failed to save {worlds.Count} entries for {nameof(WorldPoco)}: Saving from {nameof(IDataSaver<WorldPoco>)} returned failure";
        _logger.Received().LogError(errorMessage);
    }

    private List<WorldPoco> SetupSave()
    {
        var saveList = new List<WorldWebPoco> { new(1, "Ramuh"), new(2, "Fenrir") };

        _saver.SaveAsync(default!).ReturnsForAnyArgs(true);
        _fetcher.GetAllAsync().Returns(saveList);

        return saveList.ToWorldPocoList();
    }
}