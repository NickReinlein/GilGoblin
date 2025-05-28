using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Accountant;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Savers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace GilGoblin.Tests.Accountant.Integration;

public class RecipeCostAccountantTests : AccountantTests<RecipeCostPoco>
{
    private ILogger<RecipeCostAccountant> _costLogger;
    private IDataSaver<RecipeCostPoco> _saver;
    private RecipeCostAccountant _accountant;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _costLogger = Substitute.For<ILogger<RecipeCostAccountant>>();
        _saver = Substitute.For<IDataSaver<RecipeCostPoco>>();
        _saver.SaveAsync(Arg.Any<IEnumerable<RecipeCostPoco>>(), Arg.Any<CancellationToken>()).ReturnsForAnyArgs(true);

        _accountant = new RecipeCostAccountant(_serviceProvider, _saver, _costLogger);
    }

    [Test]
    public async Task GetIdsToUpdate_WhenCostsAreExpired_ThenWeReturnExpiredCosts([Values] bool isExpired)
    {
        const int entityCount = 30;
        SetupReposToReturnXEntities(entityCount);
        if (isExpired)
            await MakeCostsOutdated();

        var result = await _accountant.GetIdsToUpdate(WorldId);

        _recipeRepo.Received(1).GetAll();
        await _costRepo.GetAllAsync(WorldId);
        Assert.That(result, Has.Count.GreaterThanOrEqualTo(isExpired ? entityCount : 0));
    }

    [TestCase(0)]
    [TestCase(-1)]
    public async Task GivenCalculateAsync_WhenTheWorldIdIsInvalid_ThenWeStopGracefully(int invalidWorldId)
    {
        await _accountant.CalculateAsync(invalidWorldId, CancellationToken.None);

        await _costRepo.DidNotReceive().GetAllAsync(invalidWorldId);
        await _saver.DidNotReceive().SaveAsync(Arg.Any<IEnumerable<RecipeCostPoco>>(), Arg.Any<CancellationToken>());
        await _calc.DidNotReceive().CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public void GivenCalculateAsync_WhenTheTokenIsCancelled_ThenWeExitGracefully()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();
        Assert.DoesNotThrowAsync(async () => await _accountant.CalculateAsync(WorldId, cts.Token));

        _saver.DidNotReceive().SaveAsync(Arg.Any<IEnumerable<RecipeCostPoco>>(), Arg.Any<CancellationToken>());
        _calc.DidNotReceive().CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public async Task GivenComputeListAsync_WhenSuccessful_ThenWeReturnAPoco()
    {
        await ResetAndRecreateDatabaseAsync();

        var cts = new CancellationTokenSource();

        cts.CancelAfter(300);
        await _accountant.ComputeListAsync(WorldId, [RecipeId], cts.Token);

        await using var costAfter = GetDbContext();
        var result = costAfter.RecipeCost
            .Where(after =>
                after.RecipeId == RecipeId &&
                after.WorldId == WorldId)
            .ToList();
        Assert.That(result, Is.Not.Null.Or.Empty);
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Count.EqualTo(2), "Expected both HQ and NQ costs to be saved");
            Assert.That(result.Any(r => r.IsHq), "Expected at least one HQ cost to be saved");
            Assert.That(result.Any(r => !r.IsHq), "Expected at least one NQ cost to be saved");
            Assert.That(result.All(r => r.WorldId == WorldId));
            Assert.That(result.All(r => r.RecipeId == RecipeId));
            Assert.That(result.All(r => r.Amount > 30));
            Assert.That(result.All(r => r.Amount < 1000));
            Assert.That(result.All(r => r.LastUpdated > DateTimeOffset.UtcNow.AddMinutes(-5)));
        });
    }

    [Test]
    public async Task GivenComputeListAsync_WhenIdsAreValidAndCostsOutdated_ThenRelevantCostsAreFetched()
    {
        await ResetAndRecreateDatabaseAsync();
        await MakeCostsOutdated();
        await using var dbContext = GetDbContext();
        var recipeIds = dbContext.Recipe.Select(r => r.Id).ToList();
        _calc.CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>())
            .ReturnsForAnyArgs(10);

        await _accountant.ComputeListAsync(WorldId, ValidRecipeIds);

        await _costRepo.Received(1).GetMultipleAsync(WorldId, ValidRecipeIds);
        _recipeRepo.Received(1).GetMultiple(ValidRecipeIds);
        foreach (var recipeId in recipeIds)
        {
            foreach (var quality in new[] { true, false })
            {
                await _calc.Received(1)
                    .CalculateCraftingCostForRecipe(
                        WorldId,
                        recipeId,
                        quality);
            }
        }

        await _saver.Received(1).SaveAsync(Arg.Any<IEnumerable<RecipeCostPoco>>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GivenComputeListAsync_WhenTaskIsCanceledByUser_ThenWeHandleTheCancellationCorrectly()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        await _accountant.ComputeListAsync(WorldId, [RecipeId], cts.Token);

        await _calc.DidNotReceive()
            .CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
        await _saver.DidNotReceive().SaveAsync(Arg.Any<IEnumerable<RecipeCostPoco>>(), Arg.Any<CancellationToken>());
        await _calc.DidNotReceive().CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenNoIdsAreReturned_ThenNoExceptionIsThrown()
    {
        _recipeRepo.GetAll().Returns([]);

        await _accountant.CalculateAsync(WorldId, CancellationToken.None);

        await _saver.DidNotReceive().SaveAsync([], Arg.Any<CancellationToken>());
        await _calc.DidNotReceive().CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenAnExceptionIsThrown_ThenWeStopGracefully()
    {
        _recipeRepo.GetAll().ThrowsForAnyArgs<DataException>();

        await _accountant.CalculateAsync(WorldId);

        await _costRepo.DidNotReceive().GetAllAsync(Arg.Any<int>());
        await _saver.DidNotReceive().SaveAsync(Arg.Any<IEnumerable<RecipeCostPoco>>(), Arg.Any<CancellationToken>());
        await _calc.DidNotReceive().CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenALargeNumberOfIdsAreReturned_ThenEntitiesAreBatchedForProcessing()
    {
        SetupReposToReturnXEntities(1000);
        await MakeCostsOutdated();
        const int expectedNumberOfBatches = 1000 / 100; // 100 entities per batch

        await _accountant.CalculateAsync(WorldId);

        await _costRepo.Received(expectedNumberOfBatches).GetMultipleAsync(WorldId, Arg.Any<IEnumerable<int>>());
        _recipeRepo.Received(expectedNumberOfBatches).GetMultiple(Arg.Any<IEnumerable<int>>());
    }

    private async Task MakeCostsOutdated()
    {
        await using var dbContext = GetDbContext();
        var costs = await dbContext.RecipeCost
            .Where(w => w.WorldId == WorldId)
            .ToListAsync();
        foreach (var cost in costs)
            cost.LastUpdated = DateTimeOffset.UtcNow.AddDays(-7);
        await dbContext.SaveChangesAsync();
        _costRepo.GetMultipleAsync(WorldId, ValidRecipeIds).Returns(costs);
    }
}