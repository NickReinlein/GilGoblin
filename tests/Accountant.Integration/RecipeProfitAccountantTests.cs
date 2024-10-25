using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Accountant;
using GilGoblin.Api.Repository;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Savers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace GilGoblin.Tests.Accountant.Integration;

public class RecipeProfitAccountantTests : AccountantTests<RecipeProfitPoco>
{
    private ILogger<RecipeProfitAccountant> _profitLogger;
    private IDataSaver<RecipeProfitPoco> _saver;

    private static readonly int _worldId = ValidWorldIds[0];
    private static readonly int _recipeId = ValidRecipeIds[0];

    [SetUp]
    public override async Task SetUp()
    {
        await base.SetUp();
        _profitLogger = Substitute.For<ILogger<RecipeProfitAccountant>>();
        _saver = Substitute.For<IDataSaver<RecipeProfitPoco>>();
        _saver.SaveAsync(default!).ReturnsForAnyArgs(true);

        _accountant = new RecipeProfitAccountant(_serviceProvider, _saver, _profitLogger);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public async Task GivenCalculateAsync_WhenTheWorldIdIsInvalid_ThenWeLogAnError(int invalidWorldId)
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        await _accountant.CalculateAsync(invalidWorldId, cts.Token);

        var message = $"Failed to get the Ids to update for world {invalidWorldId}: World Id is invalid";
        _logger.Received().LogError(message);
    }

    [Test]
    public void GivenCalculateAsync_WhenTheTokenIsCancelled_ThenWeExitGracefully()
    {
        var infoMessage = $"Cancellation of the task by the user. Putting away the books for {_worldId}";

        var cts = new CancellationTokenSource();
        cts.Cancel();
        Assert.DoesNotThrowAsync(async () => await _accountant.CalculateAsync(_worldId, cts.Token));
    }

    [Test]
    public async Task GivenCalculateAsync_WhenUpdatesAreSuccessful_ThenWeLogSuccess()
    {
        var message = $"Boss, books are closed for world {_worldId}";

        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        await _accountant.CalculateAsync(_worldId, cts.Token);

        _logger.Received().LogInformation(message);
    }

    [Test]
    public async Task GivenComputeAsync_WhenSuccessful_ThenWeReturnAPoco()
    {
        const int millisecondsDelay = 200;

        var cts = new CancellationTokenSource();
        cts.CancelAfter(millisecondsDelay);
        await _accountant.ComputeListAsync(_worldId, [_recipeId], cts.Token);

        await using var dbContext = GetDbContext();
        var profits = dbContext.RecipeProfit
            .Where(p =>
                p.RecipeId == _recipeId &&
                p.WorldId == _worldId)
            .ToList();
        Assert.Multiple(() =>
        {
            Assert.That(profits, Is.Not.Null.Or.Empty);
            Assert.That(profits, Has.Count.EqualTo(2));
            foreach (var profit in profits)
            {
                Assert.That(profit.WorldId, Is.EqualTo(_worldId));
                Assert.That(profit.RecipeId, Is.EqualTo(_recipeId));
                Assert.That(profit.Amount, Is.GreaterThan(20));
                var totalMs = (profit.LastUpdated - DateTimeOffset.UtcNow).TotalMilliseconds;
                Assert.That(totalMs, Is.LessThan(millisecondsDelay));
            }
        });
    }

    [Test]
    public async Task GivenComputeListAsync_WhenTaskIsCanceledByUser_ThenWeStopGracefully()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        await _accountant.ComputeListAsync(_worldId, [_recipeId], cts.Token);

        _priceRepo.DidNotReceive().GetAll(Arg.Any<int>());
        _priceRepo.DidNotReceive().GetMultiple(Arg.Any<int>(), Arg.Any<IEnumerable<int>>(), Arg.Any<bool>());
        _priceRepo.DidNotReceive().Get(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public async Task GivenComputeAsync_WhenAnUnexpectedExceptionOccurs_ThenWeLogTheError()
    {
        var message = $"An unexpected exception occured during the accounting process for world {_worldId}: test123";
        _serviceProvider
            .GetService(typeof(IRecipeRepository))
            .Throws(new NotImplementedException("test123"));

        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        await _accountant.CalculateAsync(_worldId, cts.Token);

        _logger.Received().LogError(message);
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenIdsAreReturned_ThenWeReturnTheList()
    {
        var ids = await _accountant.GetIdsToUpdate(_worldId);

        Assert.Multiple(() =>
        {
            Assert.That(ids, Has.Count.GreaterThanOrEqualTo(3));
            Assert.That(ids, Does.Contain(9841));
            Assert.That(ids, Does.Contain(8854));
            Assert.That(ids, Does.Contain(11));
        });
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenNoIdsAreReturned_ThenWeLogTheInfoAndStop()
    {
        _recipeCostRepo.GetAllAsync(_worldId).Returns([]);
        _priceRepo.GetAll(_worldId).Returns([]);
        var messageInfo = $"Nothing to calculate for {_worldId}";

        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        await _accountant.CalculateAsync(_worldId, cts.Token);

        _priceRepo.Received(1).GetAll(_worldId);
        _logger.Received().LogInformation(messageInfo);
        await _saver.DidNotReceive().SaveAsync(Arg.Any<IEnumerable<RecipeProfitPoco>>(), Arg.Any<CancellationToken>());
        await _calc.DidNotReceive().CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenAnExceptionIsThrown_ThenWeLogTheErrorAndStop()
    {
        _serviceProvider.GetService(typeof(IRecipeRepository)).Throws<IndexOutOfRangeException>();
        var messageError =
            $"An unexpected exception occured during the accounting process for world {_worldId}: Index was outside the bounds of the array.";

        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        await _accountant.CalculateAsync(_worldId, cts.Token);

        _priceRepo.Received(1).GetAll(_worldId);
        _logger.Received().LogError(messageError);
        await _saver.DidNotReceive().SaveAsync(Arg.Any<IEnumerable<RecipeProfitPoco>>(), Arg.Any<CancellationToken>());
        await _calc.DidNotReceive().CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }
}