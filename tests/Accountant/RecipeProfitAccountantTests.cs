using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Accountant;
using GilGoblin.Api.Crafting;
using GilGoblin.Api.Repository;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace GilGoblin.Tests.Accountant;

[Timeout(10000)]
public class RecipeProfitAccountantTests : InMemoryTestDb
{
    private RecipeProfitAccountant _accountant;
    private ILogger<RecipeProfitAccountant> _logger;

    private IServiceScopeFactory _scopeFactory;
    private IServiceScope _scope;
    private IServiceProvider _serviceProvider;
    private TestGilGoblinDbContext _dbContext;
    private ICraftingCalculator _calc;
    private IRecipeProfitRepository _recipeProfitRepo;
    private IRecipeRepository _recipeRepo;
    private IPriceRepository<PricePoco> _priceRepo;
    private IRecipeCostRepository _costRepo;
    private const int worldId = 34;
    private const int recipeId = 11;
    private const int recipeId2 = 12;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _dbContext = new TestGilGoblinDbContext(_options, _configuration);
        _scopeFactory = Substitute.For<IServiceScopeFactory>();
        _logger = Substitute.For<ILogger<RecipeProfitAccountant>>();
        _scope = Substitute.For<IServiceScope>();
        _serviceProvider = Substitute.For<IServiceProvider>();
        _calc = Substitute.For<ICraftingCalculator>();
        _recipeProfitRepo = Substitute.For<IRecipeProfitRepository>();
        _recipeRepo = Substitute.For<IRecipeRepository>();
        _priceRepo = Substitute.For<IPriceRepository<PricePoco>>();
        _costRepo = Substitute.For<IRecipeCostRepository>();

        _scopeFactory.CreateScope().Returns(_scope);
        _scope.ServiceProvider.Returns(_serviceProvider);
        _serviceProvider.GetService(typeof(GilGoblinDbContext)).Returns(_dbContext);
        _serviceProvider.GetService(typeof(ICraftingCalculator)).Returns(_calc);
        _serviceProvider.GetService(typeof(IRecipeProfitRepository)).Returns(_recipeProfitRepo);
        _serviceProvider.GetService(typeof(IRecipeRepository)).Returns(_recipeRepo);
        _serviceProvider.GetService(typeof(IPriceRepository<PricePoco>)).Returns(_priceRepo);
        _serviceProvider.GetService(typeof(IRecipeCostRepository)).Returns(_costRepo);

        _recipeRepo.GetAll().Returns(_dbContext.Recipe.ToList());
        _recipeRepo
            .GetMultiple(Arg.Any<IEnumerable<int>>())
            .Returns(_dbContext.Recipe.ToList());
        _recipeProfitRepo.GetAll(worldId).Returns(_dbContext.RecipeProfit.ToList());
        _priceRepo.GetAll(worldId).Returns(_dbContext.Price.ToList());

        _accountant = new RecipeProfitAccountant(_scopeFactory, _logger);
    }

    [TestCase(0)]
    [TestCase(-1)]
    public async Task GivenCalculateAsync_WhenTheWorldIdIsInvalid_ThenWeLogAnError(int invalidWorldId)
    {
        await _accountant.CalculateAsync(CancellationToken.None, invalidWorldId);

        var message = $"Failed to get the Ids to update for world {invalidWorldId}: World Id is invalid";
        _logger.Received().LogError(message);
    }

    [Test]
    public void GivenCalculateAsync_WhenTheTokenIsCancelled_ThenWeExitGracefullyAndLogAnInfoMessage()
    {
        var infoMessage = $"Cancellation of the task by the user. Putting away the books for {worldId}";

        var cts = new CancellationTokenSource();
        cts.Cancel();
        Assert.DoesNotThrowAsync(async () => await _accountant.CalculateAsync(cts.Token, worldId));

        _logger.Received().LogInformation(infoMessage);
    }

    [Test]
    public async Task GivenCalculateAsync_WhenUpdatesAreSuccessful_ThenWeLogSuccess()
    {
        var message = $"Boss, books are closed for world {worldId}";

        var cts = new CancellationTokenSource();
        cts.CancelAfter(2000);
        await _accountant.CalculateAsync(CancellationToken.None, worldId);

        _logger.Received().LogInformation(message);
    }

    [Test]
    public async Task GivenComputeAsync_WhenSuccessful_ThenWeReturnAPoco()
    {
        const int expectedProfit = 107;
        var cts = new CancellationTokenSource();
        cts.CancelAfter(2000);
        await _accountant.ComputeListAsync(worldId, new List<int> { recipeId }, CancellationToken.None);
        await using var profitAfter = new TestGilGoblinDbContext(_options, _configuration);

        var result = profitAfter.RecipeProfit.FirstOrDefault(after => after.RecipeId == recipeId);

        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.WorldId, Is.EqualTo(worldId));
            Assert.That(result.RecipeId, Is.EqualTo(recipeId));
            Assert.That(result.RecipeProfitVsListings, Is.EqualTo(expectedProfit));
            var totalMs = (result.Updated - DateTimeOffset.UtcNow).TotalMilliseconds;
            Assert.That(totalMs, Is.LessThan(5000));
        });
    }

    [Test]
    public async Task GivenComputeListAsync_WhenNewProfitsAreCalculated_ThenWeAddItToTheRepo()
    {
        var idList = new List<int> { recipeId, recipeId2 };
        var costs = _dbContext.RecipeCost.Where(i => i.WorldId == worldId).ToList();
        _costRepo.GetAll(worldId).Returns(costs);
        _recipeProfitRepo.GetAll(worldId).Returns(new List<RecipeProfitPoco>());
        _priceRepo.GetMultiple(worldId, idList).Returns(_dbContext.Price.Where(p => p.WorldId == worldId).ToList());
        await _accountant.ComputeListAsync(worldId, idList, CancellationToken.None);

        await _recipeProfitRepo.Received(1).Add(Arg.Is<RecipeProfitPoco>(r =>
            r.RecipeId == recipeId &&
            r.WorldId == worldId));
        await _recipeProfitRepo.DidNotReceive().Add(Arg.Is<RecipeProfitPoco>(r =>
            r.RecipeId == recipeId2 &&
            r.WorldId == worldId));
    }

    [Test]
    public async Task GivenComputeListAsync_WhenNoNewProfitsAreCalculated_ThenWeDontAddToTheRepo()
    {
        var idList = new List<int> { recipeId };

        await _accountant.ComputeListAsync(worldId, idList, CancellationToken.None);

        await _recipeProfitRepo.DidNotReceive().Add(Arg.Is<RecipeProfitPoco>(r => r.RecipeId == recipeId));
    }

    [Test]
    public async Task GivenComputeListAsync_WhenTaskIsCanceledByUser_ThenWeHandleTheCancellationCorrectly()
    {
        const string message = "Task was cancelled by user. Putting away the books, boss!";

        var cts = new CancellationTokenSource();
        cts.Cancel();
        await _accountant.ComputeListAsync(worldId, new List<int> { recipeId }, cts.Token);

        await _recipeProfitRepo.DidNotReceive().Add(Arg.Any<RecipeProfitPoco>());
        _logger.Received().LogInformation(message);
    }

    [Test]
    public void GivenGetDataFreshnessInHours_WhenReceivingAResponse_ThenWeHaveAValidNumberOfHours()
    {
        var hours = RecipeProfitAccountant.GetDataFreshnessInHours().TotalHours;

        Assert.That(hours, Is.GreaterThan(0));
        Assert.That(hours, Is.LessThan(1000));
    }

    [Test]
    public async Task GivenComputeAsync_WhenAnUnexpectedExceptionOccurs_ThenWeLogTheError()
    {
        var message = $"An unexpected exception occured during the accounting process for world {worldId}: test123";
        _scope.ServiceProvider.GetRequiredService(typeof(IRecipeRepository))
            .Throws(new NotImplementedException("test123"));

        var cts = new CancellationTokenSource();
        cts.CancelAfter(2000);
        await _accountant.CalculateAsync(CancellationToken.None, worldId);

        _logger.Received().LogError(message);
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenNoIdsAreReturned_ThenWeLogTheInfoAndStop()
    {
        _costRepo.GetAll(worldId).Returns(new List<RecipeCostPoco>());
        _priceRepo.GetAll(worldId).Returns(new List<PricePoco>());
        var messageInfo = $"Nothing to calculate for {worldId}";

        await _accountant.CalculateAsync(CancellationToken.None, worldId);

        _logger.Received().LogInformation(messageInfo);
        await _recipeProfitRepo.DidNotReceive().Add(Arg.Any<RecipeProfitPoco>());
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenAnExceptionIsThrown_ThenWeLogTheErrorAndStop()
    {
        _scope.ServiceProvider.GetRequiredService(typeof(IRecipeRepository)).Throws<IndexOutOfRangeException>();
        var messageError =
            $"An unexpected exception occured during the accounting process for world {worldId}: Index was outside the bounds of the array.";

        await _accountant.CalculateAsync(CancellationToken.None, worldId);

        _logger.Received().LogError(messageError);
        await _recipeProfitRepo.DidNotReceive().Add(Arg.Any<RecipeProfitPoco>());
    }
}