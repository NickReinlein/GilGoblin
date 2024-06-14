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
using GilGoblin.Tests.InMemoryTest;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;

namespace GilGoblin.Tests.Accountant;

public class RecipeCostAccountantTests : InMemoryTestDb
{
    private RecipeCostAccountant _accountant;
    private ILogger<RecipeCostAccountant> _logger;

    private IServiceScopeFactory _scopeFactory;
    private IServiceScope _scope;
    private IServiceProvider _serviceProvider;
    private TestGilGoblinDbContext _dbContext;
    private ICraftingCalculator _calc;
    private IRecipeCostRepository _recipeCostRepo;
    private IRecipeRepository _recipeRepo;
    private IPriceRepository<PricePoco> _priceRepo;
    private const int WorldId = 34;
    private const int RecipeId = 11;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _dbContext = new TestGilGoblinDbContext(_options, _configuration);
        _scopeFactory = Substitute.For<IServiceScopeFactory>();
        _logger = Substitute.For<ILogger<RecipeCostAccountant>>();
        _scope = Substitute.For<IServiceScope>();
        _serviceProvider = Substitute.For<IServiceProvider>();
        _calc = Substitute.For<ICraftingCalculator>();
        _recipeCostRepo = Substitute.For<IRecipeCostRepository>();
        _recipeRepo = Substitute.For<IRecipeRepository>();
        _priceRepo = Substitute.For<IPriceRepository<PricePoco>>();

        _scopeFactory.CreateScope().Returns(_scope);
        _scope.ServiceProvider.Returns(_serviceProvider);
        _serviceProvider.GetService(typeof(GilGoblinDbContext)).Returns(_dbContext);
        _serviceProvider.GetService(typeof(ICraftingCalculator)).Returns(_calc);
        _serviceProvider.GetService(typeof(IRecipeCostRepository)).Returns(_recipeCostRepo);
        _serviceProvider.GetService(typeof(IRecipeRepository)).Returns(_recipeRepo);
        _serviceProvider.GetService(typeof(IPriceRepository<PricePoco>)).Returns(_priceRepo);

        _recipeRepo.GetAll().Returns(_dbContext.Recipe.ToList());
        _recipeRepo
            .GetMultiple(Arg.Any<IEnumerable<int>>())
            .Returns(_dbContext.Recipe.ToList());
        _recipeCostRepo.GetAll(WorldId).Returns(_dbContext.RecipeCost.ToList());
        _priceRepo.GetAll(WorldId).Returns(_dbContext.Price.ToList());

        _accountant = new RecipeCostAccountant(_scopeFactory, _logger);
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
        var infoMessage = $"Cancellation of the task by the user. Putting away the books for {WorldId}";

        var cts = new CancellationTokenSource();
        cts.Cancel();
        Assert.DoesNotThrowAsync(async () => await _accountant.CalculateAsync(cts.Token, WorldId));

        _logger.Received().LogInformation(infoMessage);
    }

    [Test]
    public async Task GivenCalculateAsync_WhenUpdatesAreSuccessful_ThenWeLogSuccess()
    {
        var message = $"Boss, books are closed for world {WorldId}";

        var cts = new CancellationTokenSource();
        cts.CancelAfter(2000);
        await _accountant.CalculateAsync(CancellationToken.None, WorldId);

        _logger.Received().LogInformation(message);
    }

    [Test]
    public async Task GivenComputeListAsync_WhenSuccessful_ThenWeReturnAPoco()
    {
        const int expectedCost = 107;

        var cts = new CancellationTokenSource();
        cts.CancelAfter(2000);
        await _accountant.ComputeListAsync(WorldId, [RecipeId], CancellationToken.None);

        await using var costAfter = new TestGilGoblinDbContext(_options, _configuration);
        var result = costAfter.RecipeCost.FirstOrDefault(after => after.RecipeId == RecipeId);
        Assert.That(result, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(result.WorldId, Is.EqualTo(WorldId));
            Assert.That(result.RecipeId, Is.EqualTo(RecipeId));
            Assert.That(result.Cost, Is.EqualTo(expectedCost));
            var totalMs = (result.Updated - DateTimeOffset.UtcNow).TotalMilliseconds;
            Assert.That(totalMs, Is.LessThan(5000));
        });
    }

    [Test]
    public async Task GivenComputeListAsync_WhenNewCostsAreCalculated_ThenWeAddEachNewToTheRepo()
    {
        await using var context = new TestGilGoblinDbContext(_options, _configuration);
        var recipeIds = context.Recipe.Select(r => r.Id).ToList();
        var costIds = context.RecipeCost.Select(r => r.RecipeId).ToList();
        var missingCostIds = recipeIds.Except(costIds).ToList();

        var cts = new CancellationTokenSource();
        cts.CancelAfter(2000);
        await _accountant.ComputeListAsync(WorldId, recipeIds, cts.Token);

        Assert.That(missingCostIds, Has.Count.GreaterThanOrEqualTo(2));
        foreach (var missingId in missingCostIds)
        {
            await _calc.Received(1).CalculateCraftingCostForRecipe(WorldId, missingId);
        }

        await _recipeCostRepo.DidNotReceive()
            .Add(Arg.Is<RecipeCostPoco>(r =>
                missingCostIds.Contains(r.RecipeId)));
    }

    [Test]
    public async Task GivenComputeListAsync_WhenNoNewCostsAreCalculated_ThenWeDontAddToTheRepo()
    {
        var idList = new List<int> { RecipeId };

        await _accountant.ComputeListAsync(WorldId, idList, CancellationToken.None);

        await _recipeCostRepo.DidNotReceive().Add(Arg.Is<RecipeCostPoco>(r => r.RecipeId == RecipeId));
    }

    [Test]
    public async Task GivenComputeListAsync_WhenTaskIsCanceledByUser_ThenWeHandleTheCancellationCorrectly()
    {
        const string message = "Task was cancelled by user. Putting away the books, boss!";

        var cts = new CancellationTokenSource();
        await cts.CancelAsync();
        await _accountant.ComputeListAsync(WorldId, [RecipeId], cts.Token);

        await _calc.DidNotReceive().CalculateCraftingCostForRecipe(Arg.Any<int>(), Arg.Any<int>());
        await _recipeCostRepo.DidNotReceive().Add(Arg.Any<RecipeCostPoco>());
        _logger.Received(1).LogInformation(message);
    }

    [Test]
    public async Task GivenComputeListAsync_WhenAnExceptionIsThrownForOneEntry_ThenWeLogTheErrorAndContinue()
    {
        var recipePocos = _dbContext.Recipe.ToList().Append(new RecipePoco { Id = 2222 }).ToList();
        _recipeRepo
            .GetMultiple(Arg.Any<IEnumerable<int>>())
            .Returns(recipePocos);
        _calc.CalculateCraftingCostForRecipe(WorldId, 2222).Throws<InvalidOperationException>();

        await _accountant.CalculateAsync(CancellationToken.None, WorldId);

        var singularFailureException = $"Failed to calculate crafting cost of recipe 2222 world {WorldId}: " +
                                       "Operation is not valid due to the current state of the object.";
        _logger.Received().LogError(singularFailureException);
        await _calc.Received(1).CalculateCraftingCostForRecipe(WorldId, 2222);
        await _calc.Received().CalculateCraftingCostForRecipe(WorldId, 33);
        var processEndedExceptionMessage =
            $"An unexpected exception occured during the accounting process for world {WorldId}: test123";
        _logger.DidNotReceive().LogError(processEndedExceptionMessage);
    }

    [Test]
    public void GivenGetDataFreshnessInHours_WhenReceivingAResponse_ThenWeHaveAValidNumberOfHours()
    {
        var hours = RecipeCostAccountant.GetDataFreshnessInHours().TotalHours;

        Assert.That(hours, Is.GreaterThan(0));
        Assert.That(hours, Is.LessThan(1000));
    }

    [Test]
    public async Task GivenCalculateAsync_WhenAnUnexpectedExceptionOccurs_ThenWeLogTheError()
    {
        var message = $"An unexpected exception occured during the accounting process for world {WorldId}: test123";
        _scope.ServiceProvider.GetRequiredService(typeof(ICraftingCalculator))
            .Throws(new NotImplementedException("test123")); //temporary

        var cts = new CancellationTokenSource();
        cts.CancelAfter(2000);
        await _accountant.CalculateAsync(CancellationToken.None, WorldId);

        _logger.Received(1).LogError(message);
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenNoIdsAreReturned_ThenWeLogTheInfoAndStop()
    {
        _scope.ServiceProvider.GetRequiredService(typeof(IRecipeRepository)).Throws<IndexOutOfRangeException>();
        var messageInfo = $"Nothing to calculate for {WorldId}";

        await _accountant.CalculateAsync(CancellationToken.None, WorldId);

        _logger.Received().LogInformation(messageInfo);
        _recipeCostRepo.DidNotReceive().GetAll(Arg.Any<int>());
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenAnExceptionIsThrown_ThenWeLogTheErrorAndStop()
    {
        _scope.ServiceProvider.GetRequiredService(typeof(IRecipeRepository)).Throws<IndexOutOfRangeException>();
        var messageError =
            $"Failed to get the Ids to update for world {WorldId}: Index was outside the bounds of the array.";

        await _accountant.CalculateAsync(CancellationToken.None, WorldId);

        _logger.Received(1).LogError(messageError);
        _recipeCostRepo.DidNotReceive().GetAll(Arg.Any<int>());
    }
}