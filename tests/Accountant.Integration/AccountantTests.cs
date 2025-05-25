using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GilGoblin.Accountant;
using GilGoblin.Api.Crafting;
using GilGoblin.Api.Repository;
using GilGoblin.Database.Pocos;
using GilGoblin.Tests.Database.Integration;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;

namespace GilGoblin.Tests.Accountant.Integration;

[TestFixture]
public abstract class AccountantTests<T> : GilGoblinDatabaseFixture where T : class, IIdentifiable
{
    protected ICraftingCalculator _calc;
    protected IRecipeCostRepository _costRepo;
    protected IRecipeRepository _recipeRepo;
    protected IPriceRepository<PricePoco> _priceRepo;
    protected IRecipeProfitRepository _profitRepo;
    protected IServiceCollection _serviceCollection;

    protected readonly int WorldId = ValidWorldIds[0];
    protected readonly int RecipeId = ValidRecipeIds[0];
    protected IServiceScope _serviceScope;

    [SetUp]
    public virtual void SetUp()
    {
        _calc = Substitute.For<ICraftingCalculator>();
        _costRepo = Substitute.For<IRecipeCostRepository>();
        _profitRepo = Substitute.For<IRecipeProfitRepository>();
        _recipeRepo = Substitute.For<IRecipeRepository>();
        _priceRepo = Substitute.For<IPriceRepository<PricePoco>>();

        _costRepo.GetMultipleAsync(WorldId, Arg.Any<IEnumerable<int>>()).Returns(GetDbContext().RecipeCost.ToList());
        _recipeRepo.GetAll().Returns(GetDbContext().Recipe.ToList());
        _recipeRepo.GetMultiple(Arg.Any<IEnumerable<int>>())
            .Returns(_ => GetDbContext().Recipe
                .Where(r => ValidRecipeIds.Contains(r.Id))
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
        _serviceProvider = new ServiceCollection().BuildServiceProvider();
        startup.ConfigureServices(_serviceCollection);

        _serviceCollection.AddSingleton(_calc);
        _serviceCollection.AddSingleton(_costRepo);
        _serviceCollection.AddSingleton(_recipeRepo);
        _serviceCollection.AddSingleton(_priceRepo);
        _serviceCollection.AddSingleton(_profitRepo);

        _serviceProvider = _serviceCollection.BuildServiceProvider();
        _serviceScope = _serviceProvider.CreateScope();
    }

    protected void SetupReposToReturnXEntities(int entityCount = 20)
    {
        RemoveExisting();

        var ids = Enumerable.Range(1, entityCount)
            .ToList();
        var recipes = ids.Select(i =>
                new RecipePoco { Id = i * 11, TargetItemId = i + 3111, CanHq = i % 2 == 0 })
            .ToList();
        var prices = ids.Select(i =>
                new PricePoco(i + 3111, WorldId, i % 2 == 0))
            .ToList();
        var costs = ids.Select(c =>
                new RecipeCostPoco(
                    c * 11,
                    WorldId,
                    c % 2 == 0,
                    c * 33,
                    DateTimeOffset.UtcNow.AddDays(-10)))
            .ToList();

        using var dbContext = GetDbContext();
        dbContext.Recipe.AddRange(recipes);
        dbContext.Price.AddRange(prices);
        dbContext.RecipeCost.AddRange(costs);
        Assert.That(dbContext.SaveChanges(), Is.GreaterThanOrEqualTo(entityCount));
        _recipeRepo.GetAll().Returns(GetDbContext().Recipe.ToList());
        _costRepo.GetAllAsync(WorldId).Returns(GetDbContext().RecipeCost.ToList());
        _costRepo.GetMultipleAsync(WorldId, Arg.Any<IEnumerable<int>>()).Returns(GetDbContext().RecipeCost.ToList());
        _priceRepo.GetAll(WorldId).Returns(GetDbContext().Price.ToList());
    }

    private void RemoveExisting()
    {
        using var removeExistingCtx = GetDbContext();
        removeExistingCtx.Recipe.RemoveRange(removeExistingCtx.Recipe);
        removeExistingCtx.Price.RemoveRange(removeExistingCtx.Price);
        removeExistingCtx.RecipeCost.RemoveRange(removeExistingCtx.RecipeCost);
        removeExistingCtx.SaveChanges();
    }
}