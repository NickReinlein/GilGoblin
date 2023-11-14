using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Accountant;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
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

    [SetUp]
    public override void SetUp()
    {
        base.SetUp();
        _dbContext = new TestGilGoblinDbContext(_options, _configuration);
        _scopeFactory = Substitute.For<IServiceScopeFactory>();
        _logger = Substitute.For<ILogger<RecipeCostAccountant>>();
        _scope = Substitute.For<IServiceScope>();
        _serviceProvider = Substitute.For<IServiceProvider>();

        _scopeFactory.CreateScope().Returns(_scope);
        _scope.ServiceProvider.Returns(_serviceProvider);
        _serviceProvider.GetService(typeof(GilGoblinDbContext)).Returns(_dbContext);
        // _serviceProvider.GetService(typeof(IDataSaver<RecipeCostPoco>)).Returns(_saver);

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
        var cts = new CancellationTokenSource();
        cts.CancelAfter(1500);
        Assert.DoesNotThrowAsync(async () => await _accountant.CalculateAsync(cts.Token, 34));

        _logger.Received().LogDebug($"Awaiting delay of {5000}ms before next batch call (Spam prevention)");
    }

    private List<RecipeCostPoco> GetPocos() => new List<RecipeCostPoco>
    {
        new() { WorldId = 34, RecipeId = 443, Cost = 339, Updated = DateTimeOffset.Now },
        new() { WorldId = 34, RecipeId = 981, Cost = 731, Updated = DateTimeOffset.Now }
    };
}