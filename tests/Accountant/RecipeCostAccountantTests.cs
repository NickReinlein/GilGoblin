using System;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Accountant;
using GilGoblin.Api.Crafting;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
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
    private PricePoco _newPrice;
    private ICraftingCalculator _calc;
    private const int worldId = 34;

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _newPrice = new PricePoco
        {
            WorldId = worldId,
            ItemId = 4421,
            AverageListingPrice = 337,
            AverageSold = 314,
            LastUploadTime = DateTimeOffset.Now.ToUnixTimeMilliseconds()
        };
        _dbContext = new TestGilGoblinDbContext(_options, _configuration);
        _scopeFactory = Substitute.For<IServiceScopeFactory>();
        _logger = Substitute.For<ILogger<RecipeCostAccountant>>();
        _scope = Substitute.For<IServiceScope>();
        _serviceProvider = Substitute.For<IServiceProvider>();
        _calc = Substitute.For<ICraftingCalculator>();

        _scopeFactory.CreateScope().Returns(_scope);
        _scope.ServiceProvider.Returns(_serviceProvider);
        _serviceProvider.GetService(typeof(GilGoblinDbContext)).Returns(_dbContext);
        _serviceProvider.GetService(typeof(ICraftingCalculator)).Returns(_calc);

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
    public async Task GivenComputeAsync_WhenAnUnexpectedExceptionOccurs_ThenWeLogTheError()
    {
        const string message = "An unexpected exception occured during accounting process: test123";
        _scope.ServiceProvider.GetRequiredService(typeof(ICraftingCalculator))
            .Throws(new NotImplementedException("test123")); //temporary

        var cts = new CancellationTokenSource();
        cts.CancelAfter(2000);
        await _accountant.CalculateAsync(CancellationToken.None, worldId);

        _logger.Received().LogError(message);
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenAnExceptionIsThrown_ThenWeLogTheError()
    {
        _scope.ServiceProvider.GetRequiredService(typeof(GilGoblinDbContext)).Throws<IndexOutOfRangeException>();
        var messageInfo = $"Nothing to calculate for {worldId}";

        await _accountant.CalculateAsync(CancellationToken.None, worldId);

        _logger.Received().LogInformation(messageInfo);
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenAnExceptionIsThrown_ThenWeLogTheErrorB()
    {
        _scope.ServiceProvider.GetRequiredService(typeof(GilGoblinDbContext)).Throws<IndexOutOfRangeException>();
        var messageError =
            $"Failed to get the Ids to update for world {worldId}: Index was outside the bounds of the array.";

        await _accountant.CalculateAsync(CancellationToken.None, worldId);

        _logger.Received().LogError(messageError);
    }

    private List<RecipeCostPoco> GetPocos() => new List<RecipeCostPoco>
    {
        new() { WorldId = worldId, RecipeId = 443, Cost = 339, Updated = DateTimeOffset.Now },
        new() { WorldId = worldId, RecipeId = 981, Cost = 731, Updated = DateTimeOffset.Now }
    };
}