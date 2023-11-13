using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Accountant;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.DataUpdater;
using GilGoblin.Fetcher;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ClearExtensions;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace GilGoblin.Tests.Accountant;

public class RecipeCostAccountantTests : InMemoryTestDb
{
    private RecipeCostAccountant _accountant;
    private IDataSaver<RecipeCostPoco> _saver;
    private ILogger<RecipeCostAccountant> _logger;

    private IServiceScopeFactory _scopeFactory;
    private IServiceScope _scope;
    private IServiceProvider _serviceProvider;
    private TestGilGoblinDbContext _dbContext;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _dbContext = new TestGilGoblinDbContext(_options, _configuration);
        _scopeFactory = Substitute.For<IServiceScopeFactory>();
        _saver = Substitute.For<IDataSaver<RecipeCostPoco>>();
        _logger = Substitute.For<ILogger<RecipeCostAccountant>>();
        _scope = Substitute.For<IServiceScope>();
        _serviceProvider = Substitute.For<IServiceProvider>();

        _scopeFactory.CreateScope().Returns(_scope);
        _scope.ServiceProvider.Returns(_serviceProvider);
        _serviceProvider.GetService(typeof(GilGoblinDbContext)).Returns(_dbContext);
        _serviceProvider.GetService(typeof(IDataSaver<RecipeCostPoco>)).Returns(_saver);

        _accountant = new RecipeCostAccountant(_scopeFactory, _logger);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public async Task GivenCalculateAsync_WhenTheWorldIdIsInvalid_ThenWeLogAnError(int worldId)
    {
        await _accountant.CalculateAsync(CancellationToken.None, worldId);

        var message = $"Failed to get the Ids to update for world {worldId}: World Id is invalid";
        _logger.Received().LogError(message);
    }

    [Test]
    public void GivenCalculateAsync_WhenTheTokenIsCancelled_ThenWeExitGracefully()
    {
        _saver.SaveAsync(default).ReturnsForAnyArgs(true);

        var cts = new CancellationTokenSource();
        cts.CancelAfter(1500);
        Assert.DoesNotThrowAsync(async () => await _accountant.CalculateAsync(cts.Token, 34));

        _logger.Received().LogDebug($"Awaiting delay of {5000}ms before next batch call (Spam prevention)");
    }

    [Test]
    public async Task GivenConvertAndSaveToDbAsync_WhenThereAreValidEntriesToSave_ThenWeConvertAndSaveThoseEntities()
    {
        var saveList = SetupSave();

        var cts = new CancellationTokenSource();
        cts.CancelAfter(1500);
        await _accountant.CalculateAsync(cts.Token, 34);

        Assert.That(saveList, Has.Count.GreaterThanOrEqualTo(2));
        await _saver.Received().SaveAsync(
            Arg.Is<List<RecipeCostPoco>>(x => saveList.All(
                s => x.Any(i =>
                    i.RecipeId == s.RecipeId &&
                    i.WorldId == s.WorldId)))
        );
    }

    [Test]
    public async Task GivenConvertAndSaveToDbAsync_WhenSavingThrowsAnException_ThenWeLogTheError()
    {
        var saveList = SetupSave();
        _saver.ClearSubstitute();
        _saver.SaveAsync(default).ThrowsAsyncForAnyArgs(new Exception("test"));

        var cts = new CancellationTokenSource();
        cts.CancelAfter(1500);
        await _accountant.CalculateAsync(cts.Token, 34);

        await _saver.ReceivedWithAnyArgs().SaveAsync(default);
        _logger.Received().LogError($"Failed to save {saveList.Count} entries for {nameof(RecipeCostPoco)}: test");
    }

    [Test]
    public async Task GivenConvertAndSaveToDbAsync_WhenSavingReturnsFalse_ThenWeLogTheError()
    {
        var saveList = SetupSave();
        _saver.ClearSubstitute();
        _saver.SaveAsync(default).Returns(false);
        var errorMessage =
            $"Failed to save {saveList.Count} entries for {nameof(RecipeCostPoco)}: Saving from {nameof(PriceSaver)} returned failure";

        var cts = new CancellationTokenSource();
        cts.CancelAfter(1500);
        await _accountant.CalculateAsync(cts.Token, 34);

        await _saver.ReceivedWithAnyArgs().SaveAsync(default);
        _logger.Received().LogError(errorMessage);
    }


    private List<RecipeCostPoco> SetupSave()
    {
        var saveList = new List<RecipeCostPoco>
        {
            new() { WorldId = 34, RecipeId = 443, Cost = 339, Updated = DateTimeOffset.Now },
            new() { WorldId = 34, RecipeId = 981, Cost = 731, Updated = DateTimeOffset.Now }
        };

        _saver.SaveAsync(default).ReturnsForAnyArgs(true);
        return saveList;
    }
}