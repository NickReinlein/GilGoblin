using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GilGoblin.Accountant;
using GilGoblin.Api.Crafting;
using GilGoblin.Api.Repository;
using GilGoblin.Database.Pocos;
using GilGoblin.Tests.IntegrationDatabaseFixture;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Accountant.Integration;

[TestFixture]
public class AccountantTests<T> : GilGoblinDatabaseFixture where T : class, IIdentifiable
{
    protected ILogger<Accountant<T>> _logger;

    protected ICraftingCalculator _calc;
    protected IRecipeCostRepository _costRepo;
    protected IRecipeRepository _recipeRepo;
    protected IPriceRepository<PricePoco> _priceRepo;
    protected IRecipeProfitRepository _profitRepo;

    [SetUp]
    public override async Task SetUp()
    {
        await base.SetUp();

        _logger = Substitute.For<ILogger<Accountant<T>>>();
        _calc = Substitute.For<ICraftingCalculator>();
        _costRepo = Substitute.For<IRecipeCostRepository>();
        _profitRepo = Substitute.For<IRecipeProfitRepository>();
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
            _priceRepo
                .GetMultiple(
                    worldId,
                    Arg.Any<IEnumerable<int>>(),
                    Arg.Any<bool>())
                .Returns(GetDbContext().Price
                    .Where(p => p.WorldId == worldId &&
                                ValidItemsIds.Contains(p.ItemId))
                    .ToList());
            _costRepo
                .GetAllAsync(worldId)
                .Returns(GetDbContext().RecipeCost
                    .Where(p => p.WorldId == worldId)
                    .ToList());
            _profitRepo
                .GetAllAsync(worldId)
                .Returns(GetDbContext().RecipeProfit
                    .Where(p => p.WorldId == worldId)
                    .ToList());
            _profitRepo
                .GetMultipleAsync(worldId, Arg.Any<IEnumerable<int>>())
                .Returns(GetDbContext().RecipeProfit
                    .Where(p =>
                        ValidRecipeIds.Contains(p.RecipeId) &&
                        p.WorldId == worldId)
                    .ToList());
        }

        var startup = new Startup(_configuration);
        var services = new ServiceCollection();
        _serviceProvider = services.BuildServiceProvider();
        startup.ConfigureServices(_serviceCollection);

        _serviceCollection.AddSingleton(_calc);
        _serviceCollection.AddSingleton(_costRepo);
        _serviceCollection.AddSingleton(_recipeRepo);
        _serviceCollection.AddSingleton(_priceRepo);
        _serviceCollection.AddSingleton(_profitRepo);

        _serviceProvider = _serviceCollection.BuildServiceProvider();
        _serviceScope = _serviceProvider.CreateScope();
    }
}