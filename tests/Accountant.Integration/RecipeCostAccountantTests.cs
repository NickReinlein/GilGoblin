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

    private static readonly int _worldId = ValidWorldIds[0];
    private static readonly int _recipeId = ValidRecipeIds[0];

    [SetUp]
    public override async Task SetUp()
    {
        await base.SetUp();
        _costLogger = Substitute.For<ILogger<RecipeCostAccountant>>();
        _saver = Substitute.For<IDataSaver<RecipeCostPoco>>();
        _saver.SaveAsync(default!).ReturnsForAnyArgs(true);

        SetupReposToReturnSpecifiedNumberOfEntities();

        _accountant = new RecipeCostAccountant(_serviceProvider, _saver, _costLogger);
    }

    [Test]
    public async Task GetIdsToUpdate_WhenCostsAreExpired_ThenWeReturnExpiredCosts()
    {
        await using var dbContext = GetDbContext();
        var recipeCount = dbContext.RecipeCost.ToList().Count;

        var result = await _accountant.GetIdsToUpdate(_worldId);

        _recipeRepo.Received(1).GetAll();
        await _costRepo.GetAllAsync(_worldId);
        Assert.That(result, Has.Count.GreaterThanOrEqualTo(recipeCount));
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
        Assert.DoesNotThrowAsync(async () => await _accountant.CalculateAsync(_worldId, cts.Token));

        _saver.DidNotReceive().SaveAsync(Arg.Any<IEnumerable<RecipeCostPoco>>(), Arg.Any<CancellationToken>());
        _calc.DidNotReceive().CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public async Task GivenComputeListAsync_WhenSuccessful_ThenWeReturnAPoco()
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        await _accountant.ComputeListAsync(_worldId, [_recipeId], cts.Token);

        await using var costAfter = GetDbContext();
        var result = costAfter.RecipeCost
            .Where(after =>
                after.RecipeId == _recipeId &&
                after.WorldId == _worldId)
            .ToList();

        Assert.That(result, Is.Not.Null.Or.Empty);
        Assert.Multiple(() =>
        {
            Assert.That(result.All(r => r.WorldId == _worldId));
            Assert.That(result.All(r => r.RecipeId == _recipeId));
            Assert.That(result.All(r => r.Amount > 30));
            Assert.That(result.All(r => r.Amount < 1000));
        });
    }

    [Test]
    public async Task GivenComputeListAsync_WhenIdsAreValidAndCostsOutdated_ThenRelevantCostsAreFetched()
    {
        await using var dbContext = GetDbContext();
        var costs = await dbContext.RecipeCost
            .Where(w => w.WorldId == _worldId)
            .ToListAsync();
        foreach (var cost in costs)
            cost.LastUpdated = DateTimeOffset.UtcNow.AddDays(-7);
        _costRepo.GetAllAsync(_worldId).Returns(costs);

        await _accountant.ComputeListAsync(_worldId, ValidRecipeIds);

        await _costRepo.Received(1).GetAllAsync(_worldId);
        _recipeRepo.Received(1).GetMultiple(ValidRecipeIds);
        foreach (var recipeId in ValidRecipeIds)
        {
            await _calc.Received(1)
                .CalculateCraftingCostForRecipe(
                    _worldId,
                    recipeId,
                    true);
            await _calc.Received(1)
                .CalculateCraftingCostForRecipe(
                    _worldId,
                    recipeId,
                    false);
        }
    }

    [Test]
    public async Task GivenComputeListAsync_WhenTaskIsCanceledByUser_ThenWeHandleTheCancellationCorrectly()
    {
        var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        await _accountant.ComputeListAsync(_worldId, [_recipeId], cts.Token);

        await _calc.DidNotReceive()
            .CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
        await _saver.DidNotReceive().SaveAsync(Arg.Any<IEnumerable<RecipeCostPoco>>(), Arg.Any<CancellationToken>());
        await _calc.DidNotReceive().CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenNoIdsAreReturned_ThenNoExceptionIsThrown()
    {
        _recipeRepo.GetAll().Returns([]);

        await _accountant.CalculateAsync(_worldId, CancellationToken.None);

        await _saver.DidNotReceive().SaveAsync([], Arg.Any<CancellationToken>());
        await _calc.DidNotReceive().CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenAnExceptionIsThrown_ThenWeStopGracefully()
    {
        _recipeRepo.GetAll().ThrowsForAnyArgs<DataException>();

        await _accountant.CalculateAsync(_worldId);

        await _costRepo.DidNotReceive().GetAllAsync(Arg.Any<int>());
        await _saver.DidNotReceive().SaveAsync(Arg.Any<IEnumerable<RecipeCostPoco>>(), Arg.Any<CancellationToken>());
        await _calc.DidNotReceive().CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenALargeNumberOfIdsAreReturned_ThenIdsAreBatchedForProcessing()
    {
        SetupReposToReturnSpecifiedNumberOfEntities(10000);

        await _accountant.CalculateAsync(_worldId);

        // TODO
        _recipeRepo.Received(1).GetMultiple(Arg.Any<IEnumerable<int>>());
        // _priceRepo.Received(1).GetMultiple(Arg.Any<int>(), Arg.Any<IEnumerable<int>>(), Arg.Any<bool>());
        // await _calc.Received(1).CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
        // await _saver.Received(1).SaveAsync(Arg.Any<IEnumerable<RecipeCostPoco>>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public void GivenGetDataFreshnessInHours_WhenCalled_ThenWeReturnTheValue()
    {
        var result = _accountant.GetDataFreshnessInHours();

        Assert.That(result, Is.GreaterThanOrEqualTo(48));
    }

    private void SetupReposToReturnSpecifiedNumberOfEntities(int count = 20)
    {
        var ids = Enumerable.Range(1, 20).ToList();
        var recipes = ids.Select(i =>
            new RecipePoco { Id = i, TargetItemId = i + 1111, CanHq = i % 2 == 0 }).ToList();
        var prices = ids.Select(i => new PricePoco(i, _worldId, i % 2 == 0)).ToList();
        var costs = ids.Select(c =>
                new RecipeCostPoco(
                    c,
                    _worldId,
                    true,
                    c * 3,
                    DateTimeOffset.UtcNow.AddDays(-7)))
            .ToList();
        _recipeRepo.GetAll().Returns(recipes);
        _priceRepo.GetAll(_worldId).Returns(prices);
        _costRepo.GetAllAsync(_worldId).Returns(costs);
    }
}