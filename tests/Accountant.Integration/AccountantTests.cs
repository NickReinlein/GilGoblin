using System;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Accountant;
using GilGoblin.Api.Repository;
using GilGoblin.Database;
using GilGoblin.Database.Pocos;
using GilGoblin.Tests.IntegrationDatabaseFixture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Accountant.Integration;

public class AccountantTests<T> : GilGoblinDatabaseFixture where T : class, IIdentifiable
{
    private Accountant<T> _accountant;
    private ILogger<Accountant<T>> _logger;

    private IServiceScopeFactory _scopeFactory;
    private IServiceScope _scope;
    // private ICraftingCalculator _calc;
    private IRecipeCostRepository _recipeCostRepo;
    private const int worldId = 34;

    [SetUp]
    public override async Task SetUp()
    {
        _scopeFactory = Substitute.For<IServiceScopeFactory>();
        _logger = Substitute.For<ILogger<Accountant<T>>>();
        _scope = Substitute.For<IServiceScope>();
        // _calc = Substitute.For<ICraftingCalculator>();
        _recipeCostRepo = Substitute.For<IRecipeCostRepository>();

        _scopeFactory.CreateScope().Returns(_scope);
        _scope.ServiceProvider.Returns(_serviceProvider);
        _serviceProvider.GetService(typeof(GilGoblinDbContext)).Returns(GetDbContext());
        // _serviceProvider.GetService(typeof(ICraftingCalculator)).Returns(_calc);
        _serviceProvider.GetService(typeof(IRecipeCostRepository)).Returns(_recipeCostRepo);

        await base.SetUp();
        _accountant = new Accountant<T>(_scopeFactory, _logger);
    }

    [Test]
    public void GivenComputeListAsync_WhenMethodIsNotImplemented_ThenWeThrowAnException()
    {
        Assert.ThrowsAsync<NotImplementedException>(async () =>
            await _accountant.ComputeListAsync(worldId, [1, 2], CancellationToken.None));
    }

    [Test]
    public void GivenGetDataFreshnessInHours_WhenMethodIsNotImplemented_ThenWeThrowAnException()
    {
        Assert.Throws<NotImplementedException>(() => _accountant.GetDataFreshnessInHours());
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenMethodIsNotImplemented_ThenWeReturnEmptyList()
    {
        var ids = await _accountant.GetIdsToUpdate(1);

        Assert.That(ids, Is.Empty);
    }
}