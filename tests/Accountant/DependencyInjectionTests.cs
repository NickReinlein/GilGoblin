using System;
using GilGoblin.Accountant;
using GilGoblin.Api.Cache;
using GilGoblin.Api.Crafting;
using GilGoblin.Api.Repository;
using GilGoblin.Database.Pocos;
using GilGoblin.Database.Savers;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using Startup = GilGoblin.Accountant.Startup;

namespace GilGoblin.Tests.Accountant;

public class AccountantDependencyInjectionTests
{
    protected WebApplicationFactory<Startup> _factory;

    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        _factory = new WebApplicationFactory<Startup>();
    }

    [Test]
    public void GivenAGoblinAccountant_WhenWeStartup_ThenWeResolveWithoutError()
    {
        var client = _factory.CreateClient();

        Assert.That(client, Is.Not.Null);
    }

    [TestCase(typeof(IAccountant))]
    [TestCase(typeof(IItemCache))]
    [TestCase(typeof(IPriceCache))]
    [TestCase(typeof(IRecipeCache))]
    [TestCase(typeof(IItemRecipeCache))]
    [TestCase(typeof(ICalculatedMetricCache<RecipeCostPoco>))]
    [TestCase(typeof(ICalculatedMetricCache<RecipeProfitPoco>))]
    [TestCase(typeof(ICraftingCalculator))]
    [TestCase(typeof(ICraftRepository))]
    [TestCase(typeof(IRecipeGrocer))]
    [TestCase(typeof(IPriceRepository<PricePoco>))]
    [TestCase(typeof(IItemRepository))]
    [TestCase(typeof(IRecipeRepository))]
    [TestCase(typeof(IRecipeCostRepository))]
    [TestCase(typeof(IRecipeProfitRepository))]
    [TestCase(typeof(IPriceSaver))]
    [TestCase(typeof(IDataSaver<RecipeCostPoco>))]
    public void GivenAGoblinAccountant_WhenWeStartup_ThenEachServiceIsResolvedSuccessfully(Type serviceType)
    {
        using var scope = _factory.Services.CreateScope();

        var scopedDependencyService = scope.ServiceProvider.GetRequiredService(serviceType);

        Assert.That(scopedDependencyService, Is.Not.Null);
    }
}