using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Api.Repository;
using GilGoblin.Database.Converters;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Savers;
using GilGoblin.DataUpdater;
using GilGoblin.Fetcher;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace GilGoblin.Tests.DataUpdater;

public class PriceUpdaterTests : DataUpdaterTests
{
    private IMarketableItemIdsFetcher _marketableIdsFetcher;
    private IPriceFetcher _priceFetcher;
    private PriceUpdater _priceUpdater;
    private IDataSaver<PricePoco> _saver;
    private IPriceRepository<PricePoco> _priceRepo;
    private ILogger<PriceUpdater> _logger;
    private IRecipeRepository _recipeRepo;
    private IPriceConverter _priceConverter;
    private IWorldRepository _worldRepo;

    private const int worldId = 34;
    private const int itemId1 = 1500;
    private const int itemId2 = 3614;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _marketableIdsFetcher = Substitute.For<IMarketableItemIdsFetcher>();
        _priceFetcher = Substitute.For<IPriceFetcher>();
        _priceRepo = Substitute.For<IPriceRepository<PricePoco>>();
        _recipeRepo = Substitute.For<IRecipeRepository>();
        _worldRepo = Substitute.For<IWorldRepository>();
        _saver = Substitute.For<IDataSaver<PricePoco>>();
        _priceConverter = Substitute.For<IPriceConverter>();
        _logger = Substitute.For<ILogger<PriceUpdater>>();

        _serviceProvider.GetService(typeof(IMarketableItemIdsFetcher)).Returns(_marketableIdsFetcher);
        _serviceProvider.GetService(typeof(IPriceFetcher)).Returns(_priceFetcher);
        _serviceProvider.GetService(typeof(IPriceRepository<PricePoco>)).Returns(_priceRepo);
        _serviceProvider.GetService(typeof(IRecipeRepository)).Returns(_recipeRepo);
        _serviceProvider.GetService(typeof(IWorldRepository)).Returns(_worldRepo);
        _serviceProvider.GetService(typeof(IDataSaver<PricePoco>)).Returns(_saver);
        _serviceProvider.GetService(typeof(IPriceConverter)).Returns(_priceConverter);

        SetupSave();

        _priceUpdater = new PriceUpdater(_serviceProvider, _logger);
    }

    [Test]
    public async Task GivenFetchAsync_WhenTokenIsCancelled_ThenWeStopGracefully()
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        var ct = cts.Token;

        await cts.CancelAsync();
        await _priceUpdater.FetchAsync(34, ct);

        await _marketableIdsFetcher.DidNotReceive().GetMarketableItemIdsAsync();
        await _priceFetcher.DidNotReceive().FetchByIdsAsync(
            Arg.Any<IEnumerable<int>>(),
            Arg.Any<int>(),
            Arg.Any<CancellationToken>());
        await _priceConverter.DidNotReceive()
            .ConvertAndSaveAsync(
                Arg.Any<PriceWebPoco>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GivenExecuteTask_WhenWorldsAreReturned_ThenWeFetchForEach()
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        var ct = cts.Token;
        var worlds = Enumerable.Range(1, 10)
            .Select(i => new WorldPoco { Id = i })
            .ToList();
        _worldRepo.GetAll().Returns(worlds);

        await _priceUpdater.StartAsync(ct);

        foreach (var world in worlds)
            await _priceFetcher.Received(1).FetchByIdsAsync(
                Arg.Any<IEnumerable<int>>(),
                world.Id,
                Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GivenFetchAsync_WhenRun_ThenWeFetchConvertAndSaveResults()
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        var ct = cts.Token;
        var idList = GetMultipleNewPocos().Select(i => i.GetId()).ToList();

        await _priceUpdater.FetchAsync(34, ct);

        await _marketableIdsFetcher.Received(1).GetMarketableItemIdsAsync();
        await _priceFetcher.Received(1).FetchByIdsAsync(
            Arg.Any<IEnumerable<int>>(),
            34,
            ct);
        foreach (var pocoId in idList)
        {
            await _priceConverter.Received(1)
                .ConvertAndSaveAsync(
                    Arg.Is<PriceWebPoco>(p => p.GetId() == pocoId),
                    worldId,
                    ct);
        }
    }

    [Test]
    public async Task GivenFetchAsync_WhenSuccessful_ThenWeSaveResults()
    {
        SetupSave();

        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        var ct = cts.Token;
        var idList = GetMultipleNewPocos().Select(i => i.GetId()).ToList();

        await _priceUpdater.FetchAsync(34, ct);

        await _marketableIdsFetcher.Received(1).GetMarketableItemIdsAsync();
        await _priceFetcher.Received(1).FetchByIdsAsync(
            Arg.Any<IEnumerable<int>>(),
            34,
            ct);
        foreach (var pocoId in idList)
            await _priceConverter.Received(1)
                .ConvertAndSaveAsync(
                    Arg.Is<PriceWebPoco>(p => p.GetId() == pocoId),
                    worldId,
                    ct);
    }

    [Test]
    public async Task GivenFetchAsync_WhenNoMarketableItemsAreReturned_ThenWeLogAnError()
    {
        var errorMessage = $"Failed to get the Ids to update for world {worldId}: Failed to fetch marketable item ids";
        _marketableIdsFetcher.GetMarketableItemIdsAsync().Returns([]);

        var cts = new CancellationTokenSource();
        cts.CancelAfter(100);
        await _priceUpdater.FetchAsync(34, cts.Token);

        await _marketableIdsFetcher.Received().GetMarketableItemIdsAsync();
        await _priceFetcher.DidNotReceive()
            .FetchByIdsAsync(
                Arg.Any<IEnumerable<int>>(),
                Arg.Any<int?>(),
                Arg.Any<CancellationToken>());
        _logger.Received().LogError(errorMessage);
    }

    [TestCase(0)]
    [TestCase(-1)]
    [TestCase(null)]
    public async Task GivenFetchAsync_WhenTheWorldIdIsInvalid_ThenWeLogAnError(int? worldIdString)
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        await _priceUpdater.FetchAsync(worldIdString, cts.Token);

        var message = $"Failed to get the Ids to update for world {worldIdString}: World Id is invalid";
        _logger.Received().LogError(message);
    }

    [Test]
    public void GivenFetchAsync_WhenTheTokenIsCancelled_ThenWeExitGracefully()
    {
        _marketableIdsFetcher.GetMarketableItemIdsAsync().Returns([443, 1420, 3500, 900]);
        _priceFetcher.GetEntriesPerPage().Returns(2);
        _priceFetcher.FetchByIdsAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
            .Returns([]);

        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        Assert.DoesNotThrowAsync(async () => await _priceUpdater.FetchAsync(34, cts.Token));
    }

    [Test]
    public async Task GivenConvertAndSaveToDbAsync_WhenThereAreValidEntriesToSave_ThenWeConvertAndSaveThoseEntities()
    {
        var saveList = SetupSave();

        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        await _priceUpdater.FetchAsync(34, cts.Token);

        Assert.That(saveList, Has.Count.GreaterThanOrEqualTo(2));
        await _priceConverter.Received(2).ConvertAndSaveAsync(Arg.Any<PriceWebPoco>(), Arg.Any<int>(),
            Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GivenConvertAndSaveToDbAsync_WhenSavingFails_ThenWeReturnNullAndLogTheFailure()
    {
        _ = SetupSave();
        _priceConverter
            .ConvertAndSaveAsync(Arg.Any<PriceWebPoco>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns((null, null));

        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        await _priceUpdater.FetchAsync(34, cts.Token);

        _logger.LogDebug(new DataException(), $"Failed to convert {nameof(PriceWebPoco)} to db format");
    }

    [Test]
    public async Task GivenConvertAndSaveToDbAsync_WhenSavingReturnsFalse_ThenWeLogTheFailure()
    {
        _ = SetupSave();
        _priceConverter.ConvertAndSaveAsync(
                Arg.Any<PriceWebPoco>(),
                Arg.Any<int>(),
                Arg.Any<CancellationToken>())
            .ThrowsAsync<DataException>();

        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        await _priceUpdater.FetchAsync(34, cts.Token);

        _logger.LogDebug(new DataException(), $"Failed to convert {nameof(PriceWebPoco)} to db format");
    }

    private List<PriceWebPoco> SetupSave()
    {
        var pricePoco = GetOldPrice();
        _priceRepo.GetAll(Arg.Any<int>()).Returns([pricePoco]);
        _recipeRepo.GetAll().Returns([]);
        _worldRepo.GetAll().Returns([new WorldPoco() { Id = worldId }]);

        var saveList = GetMultipleNewPocos();
        var idList = saveList.Select(i => i.GetId()).ToList();
        _marketableIdsFetcher
            .GetMarketableItemIdsAsync()
            .Returns(idList);
        _saver.SaveAsync(default!).ReturnsForAnyArgs(true);
        _priceFetcher.GetEntriesPerPage().Returns(10);
        _priceFetcher
            .FetchByIdsAsync(Arg.Any<IEnumerable<int>>(), Arg.Any<int?>(), Arg.Any<CancellationToken>())
            .Returns(saveList);
        return saveList;
    }

    private static PricePoco GetOldPrice()
    {
        return new PricePoco(itemId1, worldId, true) { Updated = DateTimeOffset.UtcNow.AddDays(-7) };
    }

    protected static List<PriceWebPoco> GetMultipleNewPocos()
    {
        var priceDataPointsPoco = new PriceDataDetailPoco(300m, worldId, DateTimeOffset.UtcNow.AddDays(-7).UtcTicks);
        var anyQ = new QualityPriceDataWebPoco(
            new PriceDataPointWebPoco(priceDataPointsPoco, priceDataPointsPoco, priceDataPointsPoco),
            new PriceDataPointWebPoco(priceDataPointsPoco, priceDataPointsPoco, priceDataPointsPoco),
            new PriceDataPointWebPoco(priceDataPointsPoco, priceDataPointsPoco, priceDataPointsPoco),
            new DailySaleVelocityWebPoco(new SaleQuantity(100m), new SaleQuantity(200m), new SaleQuantity(300m)));
        var worldUploadTimestampPocos = new List<WorldUploadTimeWebPoco>
        {
            new(worldId, 67554), new(worldId + 1, 67555)
        };

        var poco1 = new PriceWebPoco(
            itemId1,
            anyQ,
            anyQ,
            worldUploadTimestampPocos
        );
        var poco2 = new PriceWebPoco(
            itemId2,
            anyQ,
            anyQ,
            worldUploadTimestampPocos
        );

        return [poco1, poco2];
    }
}