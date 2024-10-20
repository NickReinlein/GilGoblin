using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Accountant;
using GilGoblin.Api.Crafting;
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
    protected ILogger<Accountant<T>> _logger;

    protected ICraftingCalculator _calc;
    protected IRecipeCostRepository _recipeCostRepo;
    protected IRecipeRepository _recipeRepo;
    protected IPriceRepository<PricePoco> _priceRepo;

    protected Accountant<T> _accountant;

    [SetUp]
    public override async Task SetUp()
    {
        await base.SetUp();

        _logger = Substitute.For<ILogger<Accountant<T>>>();
        _calc = Substitute.For<ICraftingCalculator>();
        _recipeCostRepo = Substitute.For<IRecipeCostRepository>();
        _recipeRepo = Substitute.For<IRecipeRepository>();
        _priceRepo = Substitute.For<IPriceRepository<PricePoco>>();

        _recipeRepo.GetAll().Returns(GetDbContext().Recipe.ToList());
        foreach (var worldId in ValidWorldIds)
        {
            _priceRepo
                .GetAll(worldId)
                .Returns(GetDbContext().Price
                    .Where(p => p.WorldId == worldId)
                    .ToList());
            _recipeCostRepo
                .GetAllAsync(worldId)
                .Returns(GetDbContext().RecipeCost
                    .Where(p => p.WorldId == worldId)
                    .ToList());
        }

        var startup = new Startup(_configuration);
        var services = new ServiceCollection();
        _serviceProvider = services.BuildServiceProvider();
        startup.ConfigureServices(_serviceCollection);

        _serviceCollection.AddSingleton(_calc);
        _serviceCollection.AddSingleton(_recipeCostRepo);
        _serviceCollection.AddSingleton(_recipeRepo);
        _serviceCollection.AddSingleton(_priceRepo);

        _serviceProvider = _serviceCollection.BuildServiceProvider();
        _serviceScope = _serviceProvider.CreateScope();

        _accountant = new Accountant<T>(_serviceProvider, _logger);
    }

    // [Test]
    // public void GivenComputeListAsync_WhenMethodIsNotImplemented_ThenWeThrowAnException()
    // {
    //     Assert.ThrowsAsync<NotImplementedException>(async () =>
    //         await _accountant.ComputeListAsync(ValidWorldIds[0], [1, 2], CancellationToken.None));
    // }
    //
    // [Test]
    // public void GivenGetDataFreshnessInHours_WhenMethodIsNotImplemented_ThenWeThrowAnException()
    // {
    //     Assert.Throws<NotImplementedException>(() => _accountant.GetDataFreshnessInHours());
    // }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenMethodIsNotImplemented_ThenWeReturnEmptyList()
    {
        var ids = await _accountant.GetIdsToUpdate(1);

        Assert.That(ids, Is.Empty);
    }
}