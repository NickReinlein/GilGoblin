using System;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Accountant;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Savers;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace GilGoblin.Tests.Accountant.Integration;

public class RecipeCostAccountantTests : AccountantTests<RecipeCostPoco>
{
    private IDataSaver<RecipeCostPoco> _saver;
    private static readonly int _worldId = ValidWorldIds[0];
    private static readonly int _recipeId = ValidRecipeIds[0];

    [SetUp]
    public override async Task SetUp()
    {
        await base.SetUp();
        _saver = Substitute.For<IDataSaver<RecipeCostPoco>>();
        _saver.SaveAsync(default!).ReturnsForAnyArgs(true);

        _accountant = new RecipeCostAccountant(_serviceProvider, _saver, _logger);
    }

    [Test]
    public async Task GetIdsToUpdate_WhenCostsAreExpired_ThenWeReturnExpiredCosts()
    {
        await using var dbContext = GetDbContext();
        var recipeCount = dbContext.RecipeCost.ToList().Count;

        var result = await _accountant.GetIdsToUpdate(_worldId);

        _priceRepo.Received(1).GetAll(_worldId);
        _recipeRepo.Received(1).GetAll();
        Assert.That(result, Has.Count.GreaterThanOrEqualTo(recipeCount));
    }


    [TestCase(0)]
    [TestCase(-1)]
    public async Task GivenCalculateAsync_WhenTheWorldIdIsInvalid_ThenWeLogAnError(int invalidWorldId)
    {
        await _accountant.CalculateAsync(invalidWorldId, CancellationToken.None);

        var message = $"Failed to get the Ids to update for world {invalidWorldId}: World Id is invalid";
        _logger.Received().LogError(message);
    }

    [Test]
    public void GivenCalculateAsync_WhenTheTokenIsCancelled_ThenWeExitGracefully()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();
        Assert.DoesNotThrowAsync(async () => await _accountant.CalculateAsync(_worldId, cts.Token));
    }

    [Test]
    public async Task GivenComputeListAsync_WhenSuccessful_ThenWeReturnAPoco()
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(500);
        await _accountant.ComputeListAsync(_worldId, [_recipeId], cts.Token);

        await using var costAfter = GetDbContext();
        var result = costAfter.RecipeCost
            .FirstOrDefault(after =>
                after.RecipeId == _recipeId &&
                after.WorldId == _worldId);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.WorldId, Is.EqualTo(_worldId));
            Assert.That(result.RecipeId, Is.EqualTo(_recipeId));
            Assert.That(result.Cost, Is.GreaterThan(1));
            var totalSeconds = (result.LastUpdated - DateTimeOffset.UtcNow).TotalSeconds;
            Assert.That(totalSeconds, Is.LessThan(3));
        });
    }

    [Test, Timeout(2000)]
    public async Task GivenComputeListAsync_WhenIdsAreValid_ThenRelevantCostsAreFetched()
    {
        await _accountant.ComputeListAsync(_worldId, ValidRecipeIds, CancellationToken.None);

        await _recipeCostRepo.Received(1).GetAllAsync(_worldId);
        _recipeRepo.Received(1).GetMultiple(ValidRecipeIds);
        await _calc.Received(ValidRecipeIds.Count)
            .CalculateCraftingCostForRecipe(
                _worldId,
                Arg.Is<int>(i => ValidRecipeIds.Contains(i)),
                Arg.Any<bool>());
    }

    [Test]
    public async Task GivenComputeListAsync_WhenTaskIsCanceledByUser_ThenWeHandleTheCancellationCorrectly()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        await _accountant.ComputeListAsync(_worldId, [_recipeId], cts.Token);

        await _calc.DidNotReceive()
            .CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public void GivenGetIdsToUpdate_WhenNoIdsAreReturned_ThenNoExceptionIsThrown()
    {
        _recipeRepo.GetAll().Returns([]);

        Assert.DoesNotThrowAsync(async () =>
            await _accountant.CalculateAsync(_worldId, CancellationToken.None));
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenAnExceptionIsThrown_ThenWeLogTheErrorAndStop()
    {
        _recipeRepo.GetAll().ThrowsForAnyArgs<DataException>();
        var messageError =
            $"Failed to get the Ids to update for world {_worldId}: Data Exception.";

        await _accountant.CalculateAsync(_worldId, CancellationToken.None);

        _logger.Received(1).LogError(messageError);
        await _recipeCostRepo.DidNotReceive().GetAllAsync(Arg.Any<int>());
    }

    [Test]
    public void GivenGetDataFreshnessInHours_WhenCalled_ThenWeReturnTheValue()
    {
        var result = _accountant.GetDataFreshnessInHours();

        Assert.That(result, Is.GreaterThanOrEqualTo(24));
    }
}