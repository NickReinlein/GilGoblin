using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Savers;
using GilGoblin.DataUpdater;
using GilGoblin.Fetcher;
using GilGoblin.Fetcher.Pocos;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace GilGoblin.Tests.DataUpdater;

public class WorldUpdaterTests : DataUpdaterTests
{
    protected WorldUpdater _worldUpdater;
    protected IWorldFetcher _fetcher;
    protected IDataSaver<WorldPoco> _saver;
    protected ILogger<WorldUpdater> _logger;
    protected List<WorldWebPoco> _worldList;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _saver = Substitute.For<IDataSaver<WorldPoco>>();
        _fetcher = Substitute.For<IWorldFetcher>();
        _logger = Substitute.For<ILogger<WorldUpdater>>();

        _serviceProvider.GetService(typeof(IWorldFetcher)).Returns(_fetcher);
        _serviceProvider.GetService(typeof(IDataSaver<WorldPoco>)).Returns(_saver);

        _worldList =
        [
            new(21, "Ravana"),
            new(22, "Bismarck"),
            new(34, "Brynhildr")
        ];

        _saver.SaveAsync(null!).ReturnsForAnyArgs(true);
        _fetcher.GetAllAsync().Returns(_worldList);

        _worldUpdater = new WorldUpdater(_serviceProvider, _logger);
    }

    [Test]
    public async Task GivenGetAllAsync_WhenNoWorldsAreReturned_ThenWeLogAnError()
    {
        _fetcher.GetAllAsync().Returns([]);

        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        await _worldUpdater.GetAllWorldsAsync();

        await _fetcher.Received(1).GetAllAsync();
        _logger.Received(1).LogError("Received empty list returned when fetching all worlds");
    }

    [Test]
    public async Task GivenGetAllAsync_WhenWorldsAreReturned_ThenWeDoNotLogAnError()
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        await _worldUpdater.GetAllWorldsAsync();

        await _fetcher.Received(1).GetAllAsync();
        _logger.DidNotReceive().LogError("Received empty list returned when fetching all worlds");
        Assert.Multiple(() =>
        {
            Assert.That(_worldList, Has.Count.EqualTo(3));
            Assert.That(_worldList.All(w => !(string.IsNullOrEmpty(w.Name) || w.Id <= 0)));
        });
    }

    [Test]
    public async Task GivenGetAllAsync_WhenAnExceptionIsThrown_ThenWeLogAnError()
    {
        var exception = new InvalidOperationException();
        _fetcher.GetAllAsync().ThrowsAsyncForAnyArgs(exception);

        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        await _worldUpdater.GetAllWorldsAsync();

        await _fetcher.Received(1).GetAllAsync();
        var errorMessage = $"Failed to fetch updates for worlds: {exception.Message}";
        _logger.ReceivedWithAnyArgs().LogError(errorMessage);
    }

    [Test]
    public async Task GivenGetAllWorldsAsync_WhenThereAreValidEntriesToSave_ThenWeTryToSaveThem()
    {
        await _worldUpdater.GetAllWorldsAsync();

        _scope.ServiceProvider.Received(1).GetService(typeof(IDataSaver<WorldPoco>));
        await _saver.Received(1).SaveAsync(Arg.Is<List<WorldPoco>>(i => i.Count == _worldList.Count &&
                                                                        i.All(w => _worldList.Any(wl =>
                                                                            wl.Name == w.Name))));
    }

    [Test]
    public async Task GivenGetAllWorldsAsync_WhenSavingThrowsAnException_ThenWeExitGracefullyAndLogTheError()
    {
        _saver.SaveAsync(Arg.Any<IEnumerable<WorldPoco>>()).ThrowsForAnyArgs(new DataException("test"));

        await _worldUpdater.GetAllWorldsAsync();

        _scope.ServiceProvider.Received(1).GetService(typeof(IDataSaver<WorldPoco>));
        await _saver.Received(1).SaveAsync(Arg.Is<List<WorldPoco>>(i => i.Count == _worldList.Count &&
                                                                        i.All(w => _worldList.Any(wl =>
                                                                            wl.Name == w.Name))));
        _logger.LogError($"Failed to save {_worldList.Count} entries for {nameof(WorldPoco)}: test");
    }

    [Test]
    public async Task GivenConvertAndSaveToDbAsync_WhenThereAreValidEntriesToSave_ThenWeConvertAndSaveThoseEntities()
    {
        await _worldUpdater.GetAllWorldsAsync();

        Assert.That(_worldList, Has.Count.GreaterThanOrEqualTo(2));
        await _saver.Received(1).SaveAsync(Arg.Is<List<WorldPoco>>(x =>
            x.All(y => y.Id > 0 && !string.IsNullOrWhiteSpace(y.Name))));
    }

    [Test]
    public async Task GivenConvertAndSaveToDbAsync_WhenSavingThrowsAnException_ThenWeLogTheError()
    {
        _saver.ClearSubstitute();
        _saver.SaveAsync(null!).ThrowsAsyncForAnyArgs(new Exception("test"));

        await _worldUpdater.GetAllWorldsAsync();

        _logger.Received(1).LogError($"Failed to save {_worldList.Count} entries for {nameof(WorldPoco)}: test");
    }

    [Test]
    public async Task GivenConvertAndSaveToDbAsync_WhenSavingReturnsFalse_ThenWeLogTheError()
    {
        _saver.SaveAsync(Arg.Any<IEnumerable<WorldPoco>>()).Returns(false);

        await _worldUpdater.GetAllWorldsAsync();

        var errorMessage =
            $"Failed to save {_worldList.Count} entries for {nameof(WorldPoco)}: Saving from {nameof(IDataSaver<WorldPoco>)} returned failure";
        _logger.ReceivedWithAnyArgs().LogError(errorMessage);
    }
}