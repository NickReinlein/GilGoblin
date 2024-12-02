using System;
using System.Collections.Generic;
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

public class RecipeProfitAccountantTests : AccountantTests<RecipeProfitPoco>
{
    private ILogger<RecipeProfitAccountant> _logger;
    private IDataSaver<RecipeProfitPoco> _saver;
    private RecipeProfitAccountant _accountant;

    private static readonly int _worldId = ValidWorldIds[0];
    private static readonly int _recipeId = ValidRecipeIds[0];
    private static readonly int _targetItemId = ValidItemsIds[0];

    [SetUp]
    public override async Task SetUp()
    {
        await base.SetUp();

        _logger = Substitute.For<ILogger<RecipeProfitAccountant>>();
        _saver = Substitute.For<IDataSaver<RecipeProfitPoco>>();
        _saver.SaveAsync(Arg.Any<IEnumerable<RecipeProfitPoco>>(), Arg.Any<CancellationToken>())
            .ReturnsForAnyArgs(true);

        _accountant = new RecipeProfitAccountant(_serviceProvider, _saver, _logger);
    }

    [Test]
    public void GivenCalculateAsync_WhenTheTokenIsCancelled_ThenProcessingStops()
    {
        var cts = new CancellationTokenSource();

        cts.Cancel();
        Assert.DoesNotThrowAsync(async () => await _accountant.CalculateAsync(_worldId, cts.Token));

        _costRepo.DidNotReceive().GetAllAsync(Arg.Any<int>());
    }

    [Test]
    public void GivenComputeListAsync_WhenTheTokenIsCancelled_ThenProcessingStops()
    {
        var cts = new CancellationTokenSource();
        cts.Cancel();
        Assert.DoesNotThrowAsync(async () => await _accountant.ComputeListAsync(_worldId, ValidItemsIds, cts.Token));

        _costRepo.DidNotReceive().GetMultipleAsync(Arg.Any<int>(), Arg.Any<IEnumerable<int>>());
        _profitRepo.DidNotReceive().GetMultipleAsync(Arg.Any<int>(), Arg.Any<IEnumerable<int>>());
        _priceRepo.DidNotReceive().GetAll(Arg.Any<int>());
        _saver.DidNotReceive().SaveAsync(Arg.Any<IEnumerable<RecipeProfitPoco>>(), cts.Token);
    }

    [Test]
    public async Task
        GivenComputeListAsync_WhenDeterminingWhichRecipesToCompute_ThenRelevantEntitiesAreFetchedAndSaved()
    {
        var idList = new List<int> { _recipeId };
        await RemoveExistingProfitEntries();
        var recipeCosts = GetCosts().ToList();
        foreach (var cost in recipeCosts)
            cost.LastUpdated = DateTimeOffset.UtcNow.AddDays(-10);
        _costRepo.GetMultipleAsync(_worldId, idList).Returns(recipeCosts);

        await _accountant.ComputeListAsync(_worldId, idList);

        await _costRepo.Received(1).GetMultipleAsync(_worldId, idList);
        await _profitRepo.Received(1).GetMultipleAsync(_worldId, idList);
        _recipeRepo.Received(1).GetMultiple(idList);
        _priceRepo.Received(1).GetAll(_worldId);
        await _saver.Received(1).SaveAsync(Arg.Any<IEnumerable<RecipeProfitPoco>>());
    }

    [TestCaseSource(nameof(ValidRecipeIds))]
    public async Task GivenComputeListAsync_WhenSuccessful_ThenProfitsAreReturned(int recipeId)
    {
        var cts = new CancellationTokenSource();
        cts.CancelAfter(200);
        await _accountant.ComputeListAsync(_worldId, [recipeId], cts.Token);

        await using var dbContext = GetDbContext();
        var profits = dbContext.RecipeProfit
            .Where(p =>
                p.RecipeId == recipeId &&
                p.WorldId == _worldId)
            .ToList();
        Assert.Multiple(() =>
        {
            Assert.That(profits, Is.Not.Null.Or.Empty);
            Assert.That(profits, Has.Count.EqualTo(2));
            foreach (var profit in profits)
            {
                Assert.That(profit.WorldId, Is.EqualTo(_worldId));
                Assert.That(profit.RecipeId, Is.EqualTo(recipeId));
                Assert.That(profit.Amount, Is.GreaterThan(20));
                var totalMs = (profit.LastUpdated - DateTimeOffset.UtcNow).TotalMilliseconds;
                Assert.That(totalMs, Is.LessThan(200));
            }
        });
    }

    [Test]
    public async Task
        GivenComputeListAsync_WhenSomeEntitiesHaveNoValidMatchingRecipe_ThenCalculationIsSkippedForThoseEntitiesAndWarningLogged()
    {
        _recipeRepo.GetMultiple(ValidRecipeIds).Returns([]);

        await _accountant.ComputeListAsync(_worldId, ValidRecipeIds);

        foreach (var recipeId in ValidRecipeIds)
        {
            _logger.Received().Log(
                LogLevel.Warning,
                Arg.Any<EventId>(),
                Arg.Is<object>(o => o.ToString() == $"Failed to find recipe {recipeId}"),
                Arg.Any<Exception>(),
                Arg.Any<Func<object, Exception, string>>()!);
        }
        await _saver.Received(1).SaveAsync(Arg.Is<IEnumerable<RecipeProfitPoco>>(p => !p.Any()));
    }

    [Test]
    public async Task GivenComputeListAsync_WhenNoCostsExist_ThenProcessingIsSkipped()
    {
        _costRepo.GetMultipleAsync(_worldId, ValidRecipeIds).Returns([]);

        await _accountant.ComputeListAsync(_worldId, ValidRecipeIds);

        await _costRepo.Received(1).GetMultipleAsync(_worldId, Arg.Any<IEnumerable<int>>());
        await _profitRepo.DidNotReceive().GetMultipleAsync(_worldId, Arg.Any<IEnumerable<int>>());
        _priceRepo.DidNotReceive().GetAll(Arg.Any<int>());
        await _saver.DidNotReceive().SaveAsync(Arg.Any<IEnumerable<RecipeProfitPoco>>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenProfitsAreExpired_ThenIdsOfExpiredProfitsAreReturned()
    {
        await using var dbContext = GetDbContext();
        var profits = await dbContext.RecipeProfit
            .Where(p => p.WorldId == _worldId)
            .ToListAsync();
        foreach (var profit in profits)
            profit.LastUpdated = DateTimeOffset.UtcNow.AddDays(-30);
        await dbContext.SaveChangesAsync();
        _profitRepo.GetMultipleAsync(_worldId, Arg.Any<IEnumerable<int>>()).Returns(profits);

        var result = await _accountant.GetIdsToUpdate(_worldId);

        Assert.That(result, Has.Count.GreaterThanOrEqualTo(profits.Count));
        await _costRepo.Received(1).GetAllAsync(_worldId);
        await _profitRepo.Received(1).GetMultipleAsync(_worldId, Arg.Any<IEnumerable<int>>());
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenNoProfitsExist_ThenAllCostsAreReturned()
    {
        const int entityCount = 7;
        SetupReposToReturnXEntities(entityCount);
        _profitRepo.GetMultipleAsync(_worldId, Arg.Any<int[]>()).Returns([]);

        var result = await _accountant.GetIdsToUpdate(_worldId);

        Assert.That(result, Has.Count.EqualTo(entityCount));
        await _costRepo.Received(1).GetAllAsync(_worldId);
        await _profitRepo.Received(1).GetMultipleAsync(_worldId, Arg.Any<IEnumerable<int>>());
    }

    [TestCase(0)]
    [TestCase(-1)]
    public async Task GivenCalculateAsync_WhenTheWorldIdIsInvalid_ThenProcessingIsSkipped(int invalidWorldId)
    {
        await _accountant.CalculateAsync(invalidWorldId);

        await _costRepo.DidNotReceive().GetAllAsync(invalidWorldId);
        _priceRepo.DidNotReceive().GetAll(invalidWorldId);
        _priceRepo.DidNotReceive().GetMultiple(Arg.Any<int>(), Arg.Any<IEnumerable<int>>(), Arg.Any<bool>());
        _priceRepo.DidNotReceive().Get(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
        await _saver.DidNotReceive().SaveAsync(Arg.Any<IEnumerable<RecipeProfitPoco>>(), Arg.Any<CancellationToken>());
        await _calc.DidNotReceive().CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenNoIdsAreReturned_ThenNoExceptionIsThrown()
    {
        _costRepo.GetAllAsync(_worldId).Returns([]);

        await _accountant.CalculateAsync(_worldId);

        await _costRepo.Received(1).GetAllAsync(_worldId);
        await _profitRepo.Received(1).GetMultipleAsync(_worldId, Arg.Is<IEnumerable<int>>(i => !i.Any()));
        _priceRepo.DidNotReceive().GetMultiple(Arg.Any<int>(), Arg.Any<IEnumerable<int>>(), Arg.Any<bool>());
        _priceRepo.DidNotReceive().Get(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
        await _saver.DidNotReceive().SaveAsync(Arg.Any<IEnumerable<RecipeProfitPoco>>(), Arg.Any<CancellationToken>());
        await _calc.DidNotReceive().CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public async Task GivenComputeListAsync_WhenCostRepoThrowsAnException_ThenProcessingIsSkipped()
    {
        _costRepo.GetMultipleAsync(_worldId, Arg.Any<IEnumerable<int>>()).ThrowsAsync<ArithmeticException>();

        await _accountant.ComputeListAsync(_worldId, ValidRecipeIds);

        await _costRepo.Received(1).GetMultipleAsync(_worldId, ValidRecipeIds);
        await _profitRepo.DidNotReceive().GetMultipleAsync(_worldId, Arg.Any<IEnumerable<int>>());
        _priceRepo.DidNotReceive().GetAll(Arg.Any<int>());
        await _saver.DidNotReceive().SaveAsync(Arg.Any<IEnumerable<RecipeProfitPoco>>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task GivenComputeListAsync_WhenProfitRepoThrowsAnException_ThenProcessingStops()
    {
        _profitRepo.GetMultipleAsync(_worldId, Arg.Any<IEnumerable<int>>()).ThrowsAsync<ArithmeticException>();

        await _accountant.ComputeListAsync(_worldId, ValidRecipeIds);

        await _costRepo.Received(1).GetMultipleAsync(_worldId, ValidRecipeIds);
        await _profitRepo.Received(1).GetMultipleAsync(_worldId, ValidRecipeIds);
        _priceRepo.DidNotReceive().GetAll(Arg.Any<int>());
        await _saver.DidNotReceive().SaveAsync(Arg.Any<IEnumerable<RecipeProfitPoco>>(), Arg.Any<CancellationToken>());
        await _calc.DidNotReceive().CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public async Task GivenCalculateAsync_WhenCostRepoThrowsAnException_ThenProcessingStops()
    {
        _costRepo.GetAllAsync(_worldId).ThrowsAsync<ArithmeticException>();

        await _accountant.CalculateAsync(_worldId);

        await _profitRepo.DidNotReceive().GetMultipleAsync(_worldId, Arg.Any<IEnumerable<int>>());
        _priceRepo.DidNotReceive().GetAll(Arg.Any<int>());
        _priceRepo.DidNotReceive().GetMultiple(Arg.Any<int>(), Arg.Any<IEnumerable<int>>(), Arg.Any<bool>());
        _priceRepo.DidNotReceive().Get(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
        await _saver.DidNotReceive().SaveAsync(Arg.Any<IEnumerable<RecipeProfitPoco>>(), Arg.Any<CancellationToken>());
        await _calc.DidNotReceive().CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public async Task GivenCalculateAsync_WhenProfitRepoThrowsAnException_ThenProcessingStops()
    {
        _profitRepo.GetMultipleAsync(_worldId, Arg.Any<IEnumerable<int>>()).ThrowsAsync<ArithmeticException>();

        await _accountant.CalculateAsync(_worldId);

        _recipeRepo.DidNotReceive().GetMultiple(Arg.Any<IEnumerable<int>>());
        _priceRepo.DidNotReceive().GetAll(Arg.Any<int>());
        _priceRepo.DidNotReceive().GetMultiple(Arg.Any<int>(), Arg.Any<IEnumerable<int>>(), Arg.Any<bool>());
        _priceRepo.DidNotReceive().Get(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
        await _saver.DidNotReceive().SaveAsync(Arg.Any<IEnumerable<RecipeProfitPoco>>(), Arg.Any<CancellationToken>());
        await _calc.DidNotReceive().CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<bool>());
    }

    [Test]
    public void GivenCalculateCraftingProfitForQualityRecipe_WhenCostsIsEmpty_ThenReturnNull()
    {
        var result = _accountant.CalculateCraftingProfitForRecipe(
            recipeId: _recipeId,
            worldId: _worldId,
            isHq: false,
            targetItemId: _targetItemId,
            costs: [],
            prices: GetPrices());

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenCalculateCraftingProfitForQualityRecipe_WhenPricesIsEmpty_ThenReturnNull()
    {
        var result = _accountant.CalculateCraftingProfitForRecipe(
            recipeId: _recipeId,
            worldId: _worldId,
            isHq: false,
            targetItemId: _targetItemId,
            costs: GetCosts(),
            prices: []);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenCalculateCraftingProfitForQualityRecipe_WhenCostAndPriceDoNotMatch_ThenReturnNull()
    {
        var costs = new List<RecipeCostPoco> { new(_recipeId, _worldId, false, 100, DateTimeOffset.UtcNow) };
        var prices = new List<PricePoco> { new(_worldId, _recipeId + 1, false) };

        var result = _accountant.CalculateCraftingProfitForRecipe(
            recipeId: _recipeId,
            worldId: _worldId,
            isHq: false,
            targetItemId: _targetItemId,
            costs: costs,
            prices: prices);

        Assert.That(result, Is.Null);
    }

    [Test]
    public void GivenCalculateCraftingProfitForQualityRecipe_WhenCostAndPriceMatch_ThenProfitIsReturned()
    {
        var averageSalePrice = new AverageSalePricePoco(_targetItemId, _worldId, false, 111, 122, 133)
        {
            DcDataPoint = new PriceDataPoco("dc", 555, _worldId, DateTimeOffset.UtcNow.Ticks)
        };
        var costs = new List<RecipeCostPoco> { new(_recipeId, _worldId, false, 133, DateTimeOffset.UtcNow) };
        var price = new PricePoco(_targetItemId, _worldId, false, averageSalePrice.Id)
        {
            AverageSalePrice = averageSalePrice
        };
        var prices = new List<PricePoco> { price };

        var result = _accountant.CalculateCraftingProfitForRecipe(
            recipeId: _recipeId,
            worldId: _worldId,
            isHq: false,
            targetItemId: _targetItemId,
            costs: costs,
            prices: prices);

        Assert.Multiple(() =>
        {
            Assert.That(result, Is.Not.Null);
            var itemSalePrice = (int)price.GetBestPriceAmount();
            Assert.That(itemSalePrice, Is.EqualTo(555));
            var expectedProfit = itemSalePrice - costs[0].Amount;
            Assert.That(expectedProfit, Is.EqualTo(422));
            Assert.That(result!.Amount, Is.EqualTo(expectedProfit));
        });
    }

    private async Task RemoveExistingProfitEntries()
    {
        await using var dbContext = GetDbContext();
        var count = dbContext.RecipeProfit.Count();
        dbContext.RecipeProfit.RemoveRange(dbContext.RecipeProfit);
        Assert.That(await dbContext.SaveChangesAsync(), Is.EqualTo(count));
    }

    private static IEnumerable<RecipeCostPoco> GetCosts()
    {
        yield return new RecipeCostPoco(_recipeId, _worldId, false, 100, DateTimeOffset.UtcNow);
        yield return new RecipeCostPoco(_recipeId, _worldId, true, 100, DateTimeOffset.UtcNow);
    }

    private static IEnumerable<PricePoco> GetPrices()
    {
        yield return new PricePoco(_targetItemId, _worldId, false);
        yield return new PricePoco(_targetItemId, _worldId, true);
    }
}