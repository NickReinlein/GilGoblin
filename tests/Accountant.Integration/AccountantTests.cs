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
        _recipeRepo.GetMultiple(Arg.Any<IEnumerable<int>>())
            .Returns(c => GetDbContext().Recipe
                .Where(r => c.Arg<IEnumerable<int>>().Contains(r.Id))
                .ToList());
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

    [Test]
    public void GivenGetDataFreshnessInHours_WhenCalled_ThenAValueIsReturned()
    {
        var result = _accountant.GetDataFreshnessInHours();

        Assert.That(result, Is.GreaterThanOrEqualTo(24));
    }

    [Test]
    public async Task GivenGetIdsToUpdate_WhenCalled_ThenAValueIsReturned()
    {
        var result = await _accountant.GetIdsToUpdate(ValidWorldIds[0]);

        Assert.That(result, Is.Not.Null.And.Not.Empty);
    }

    [TestCaseSource(nameof(ValidWorldIds))]
    public async Task GivenComputeListAsync_WhenCalled_ThenAValueIsReturned(int worldId)
    {
        await _accountant.ComputeListAsync(worldId, ValidRecipeIds, CancellationToken.None);

        foreach (var recipeId in ValidRecipeIds)
            await _calc.Received(1).CalculateCraftingCostForRecipe(worldId, recipeId, true);
    }
}